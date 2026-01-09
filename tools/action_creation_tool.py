#!/usr/bin/env python3
import argparse
import json
import sys
from pathlib import Path

import yaml  # PyYAML
from PySide6.QtWidgets import (
    QApplication,
    QCheckBox,
    QComboBox,
    QDoubleSpinBox,
    QHBoxLayout,
    QLabel,
    QLineEdit,
    QListWidget,
    QMessageBox,
    QPushButton,
    QSpinBox,
    QVBoxLayout,
    QWidget,
)


def ensure_yaml_available(parent=None) -> bool:
    if yaml is not None:
        return True
    QMessageBox.critical(
        parent,
        "Missing dependency",
        "PyYAML is required.\n\nInstall: python -m pip install pyyaml",
    )
    return False


def load_schema(schema_path: str):
    with open(schema_path, "r", encoding="utf-8") as f:
        raw = json.load(f)
    target_rules = raw.get("target_rule", [])
    schema = {
        effect_id: effect_def.get("fields", [])
        for effect_id, effect_def in raw.get("effects", {}).items()
    }
    return target_rules, schema


def effect_summary(effect: dict) -> str:
    et = effect.get("type", "")
    fields = {k: v for k, v in effect.items() if k != "type"}
    if not fields:
        return et
    parts = []
    for k, v in fields.items():
        parts.append(f"{k}={v}")
    return f"{et}  " + ", ".join(parts)


def safe_read_yaml(path: Path) -> dict:
    if not path.exists():
        return {"actions": []}
    with open(path, "r", encoding="utf-8") as f:
        data = yaml.safe_load(f) or {}
    if not isinstance(data, dict):
        return {"actions": []}
    if "actions" not in data or not isinstance(data["actions"], list):
        data["actions"] = []
    return data


def safe_write_yaml(path: Path, data: dict):
    with open(path, "w", encoding="utf-8") as f:
        yaml.safe_dump(
            data,
            f,
            sort_keys=False,
            default_flow_style=False,
            allow_unicode=True,
            width=120,
        )


class App(QWidget):
    def __init__(self, schema_path: str, yaml_path: str):
        super().__init__()
        self.setWindowTitle("Effect YAML Tool")

        self.schema_path = schema_path
        self.yaml_path = Path(yaml_path)

        self.target_rules, self.schema = load_schema(self.schema_path)

        self.doc = {"actions": []}  # loaded YAML root
        self.selected_action_index: int | None = None
        self.selected_effect_index: int | None = None

        root = QHBoxLayout(self)

        # Left: actions list
        left = QVBoxLayout()
        left.addWidget(QLabel("Actions"))
        self.actions_list = QListWidget()
        self.actions_list.currentRowChanged.connect(self.on_action_selected)
        left.addWidget(self.actions_list, 1)

        btn_row = QHBoxLayout()
        self.new_action_btn = QPushButton("New")
        self.new_action_btn.clicked.connect(self.new_action)
        self.del_action_btn = QPushButton("Delete")
        self.del_action_btn.clicked.connect(self.delete_action)
        self.reload_btn = QPushButton("Reload YAML")
        self.reload_btn.clicked.connect(self.load_yaml)
        btn_row.addWidget(self.new_action_btn)
        btn_row.addWidget(self.del_action_btn)
        btn_row.addWidget(self.reload_btn)
        left.addLayout(btn_row)

        self.save_btn = QPushButton("Save YAML")
        self.save_btn.clicked.connect(self.save_yaml)
        left.addWidget(self.save_btn)

        root.addLayout(left, 1)

        # Right: editor
        right = QVBoxLayout()

        right.addWidget(QLabel("Action"))
        self.action_id = QLineEdit("")
        self.action_name = QLineEdit("")
        self.target_rule_combo = QComboBox()
        self.target_rule_combo.addItems(self.target_rules)
        right.addWidget(QLabel("id"))
        right.addWidget(self.action_id)
        right.addWidget(QLabel("name"))
        right.addWidget(self.action_name)
        right.addWidget(QLabel("target_rule"))
        right.addWidget(self.target_rule_combo)

        right.addWidget(QLabel("Effect Editor"))
        self.effect_type = QComboBox()
        self.effect_type.addItems(sorted(self.schema.keys()))
        self.effect_type.currentTextChanged.connect(self.rebuild_form)
        right.addWidget(self.effect_type)

        self.form_box = QVBoxLayout()
        right.addLayout(self.form_box)

        eff_btns = QHBoxLayout()
        self.add_eff_btn = QPushButton("Add Effect")
        self.add_eff_btn.clicked.connect(self.add_effect)
        self.replace_eff_btn = QPushButton("Replace Selected Effect")
        self.replace_eff_btn.clicked.connect(self.replace_selected_effect)
        self.del_eff_btn = QPushButton("Remove Selected Effect")
        self.del_eff_btn.clicked.connect(self.remove_selected_effect)
        eff_btns.addWidget(self.add_eff_btn)
        eff_btns.addWidget(self.replace_eff_btn)
        eff_btns.addWidget(self.del_eff_btn)
        right.addLayout(eff_btns)

        right.addWidget(QLabel("Current Action Effects"))
        self.effects_list = QListWidget()
        self.effects_list.currentRowChanged.connect(self.on_effect_selected)
        right.addWidget(self.effects_list, 1)

        self.commit_action_btn = QPushButton("Commit Action (Replace/Add)")
        self.commit_action_btn.clicked.connect(self.commit_action)
        right.addWidget(self.commit_action_btn)

        root.addLayout(right, 2)

        self.rebuild_form(self.effect_type.currentText())
        self.load_yaml()

    # ---------- YAML IO ----------
    def load_yaml(self):
        if not ensure_yaml_available(self):
            return
        self.doc = safe_read_yaml(self.yaml_path)
        self.refresh_actions_list()
        self.new_action()  # clears editor

    def save_yaml(self):
        if not ensure_yaml_available(self):
            return
        safe_write_yaml(self.yaml_path, self.doc)
        QMessageBox.information(self, "Saved", f"Saved to:\n{self.yaml_path}")

    # ---------- UI helpers ----------
    def clear_layout(self, layout):
        while layout.count():
            item = layout.takeAt(0)
            w = item.widget()
            if w is not None:
                w.deleteLater()
                continue
            child_layout = item.layout()
            if child_layout is not None:
                self.clear_layout(child_layout)
                child_layout.deleteLater()

    def rebuild_form(self, effect_type: str):
        self.clear_layout(self.form_box)
        self.field_widgets = {}

        for f in self.schema.get(effect_type, []):
            key = f["key"]
            kind = f["kind"]
            row = QHBoxLayout()
            row.addWidget(QLabel(key))

            if kind == "bool":
                w = QCheckBox()
                w.setChecked(bool(f.get("default", False)))
            elif kind == "float":
                w = QDoubleSpinBox()
                w.setRange(-1e9, 1e9)
                w.setDecimals(6)
                w.setValue(float(f.get("default", 0.0)))
            elif kind == "int":
                w = QSpinBox()
                w.setRange(-2_000_000_000, 2_000_000_000)
                w.setValue(int(f.get("default", 0)))
            elif kind == "enum":
                w = QComboBox()
                w.addItems(f["values"])
                d = f.get("default")
                if d in f["values"]:
                    w.setCurrentText(d)
            else:  # string
                w = QLineEdit(str(f.get("default", "")))

            row.addWidget(w, 1)
            self.form_box.addLayout(row)
            self.field_widgets[key] = (kind, w)

    def refresh_actions_list(self):
        self.actions_list.blockSignals(True)
        self.actions_list.clear()
        for a in self.doc.get("actions", []):
            aid = a.get("id", "")
            nm = a.get("name", "")
            self.actions_list.addItem(f"{aid}  —  {nm}")
        self.actions_list.blockSignals(False)

    def refresh_effects_list(self, effects: list[dict]):
        self.effects_list.blockSignals(True)
        self.effects_list.clear()
        for eff in effects:
            item_text = effect_summary(eff)
            item = self.effects_list.addItem(item_text)
        self.effects_list.blockSignals(False)

        # Store structured effects on the widget for easy access
        self.effects_list._effects_data = effects  # type: ignore[attr-defined]

    def current_effects(self) -> list[dict]:
        return getattr(self.effects_list, "_effects_data", [])

    # ---------- Actions ----------
    def new_action(self):
        self.selected_action_index = None
        self.actions_list.setCurrentRow(-1)
        self.action_id.setText("")
        self.action_name.setText("")
        if self.target_rules:
            self.target_rule_combo.setCurrentIndex(0)
        self.refresh_effects_list([])
        self.selected_effect_index = None
        self.effects_list.setCurrentRow(-1)

    def on_action_selected(self, row: int):
        if row < 0:
            return
        actions = self.doc.get("actions", [])
        if row >= len(actions):
            return
        a = actions[row]
        self.selected_action_index = row
        self.action_id.setText(str(a.get("id", "")))
        self.action_name.setText(str(a.get("name", "")))
        tr = str(a.get("target_rule", ""))
        if tr in self.target_rules:
            self.target_rule_combo.setCurrentText(tr)
        elif self.target_rules:
            self.target_rule_combo.setCurrentIndex(0)

        effects = a.get("effects", [])
        if not isinstance(effects, list):
            effects = []
        # Normalize each effect to dict
        norm = []
        for eff in effects:
            if isinstance(eff, dict) and "type" in eff:
                norm.append(eff)
        self.refresh_effects_list(norm)
        self.selected_effect_index = None
        self.effects_list.setCurrentRow(-1)

    def commit_action(self):
        aid = self.action_id.text().strip()
        nm = self.action_name.text().strip()
        tr = self.target_rule_combo.currentText()
        effects = self.current_effects()

        if not aid:
            QMessageBox.warning(self, "Missing id", "Action id is required.")
            return
        if not nm:
            QMessageBox.warning(self, "Missing name", "Action name is required.")
            return
        if not effects:
            QMessageBox.warning(self, "No effects", "Add at least one effect.")
            return

        action_obj = {
            "id": aid,
            "name": nm,
            "target_rule": tr,
            "effects": effects,
        }

        actions = self.doc.setdefault("actions", [])
        if self.selected_action_index is None:
            actions.append(action_obj)
            self.selected_action_index = len(actions) - 1
        else:
            actions[self.selected_action_index] = action_obj

        self.refresh_actions_list()
        self.actions_list.setCurrentRow(self.selected_action_index)

    def delete_action(self):
        row = self.actions_list.currentRow()
        if row < 0:
            return
        actions = self.doc.get("actions", [])
        if row >= len(actions):
            return
        del actions[row]
        self.refresh_actions_list()
        self.new_action()

    # ---------- Effects ----------
    def read_effect_from_form(self) -> dict:
        et = self.effect_type.currentText()
        eff = {"type": et}
        for key, (kind, w) in self.field_widgets.items():
            if kind == "bool":
                eff[key] = bool(w.isChecked())  # pyright: ignore[reportArgumentType]
            elif kind == "float":
                eff[key] = float(w.value())  # pyright: ignore[reportArgumentType]
            elif kind == "int":
                eff[key] = int(w.value())  # pyright: ignore[reportArgumentType]
            elif kind == "enum":
                eff[key] = w.currentText()
            else:
                eff[key] = w.text()
        return eff

    def apply_effect_to_form(self, eff: dict):
        et = eff.get("type", "")
        if et and et in self.schema:
            self.effect_type.setCurrentText(et)
        # after rebuild_form, set values
        for key, (kind, w) in self.field_widgets.items():
            if key not in eff:
                continue
            v = eff[key]
            if kind == "bool":
                w.setChecked(bool(v))
            elif kind == "float":
                w.setValue(float(v))
            elif kind == "int":
                w.setValue(int(v))
            elif kind == "enum":
                w.setCurrentText(str(v))
            else:
                w.setText(str(v))

    def add_effect(self):
        effects = list(self.current_effects())
        effects.append(self.read_effect_from_form())
        self.refresh_effects_list(effects)
        self.effects_list.setCurrentRow(len(effects) - 1)

    def on_effect_selected(self, row: int):
        if row < 0:
            self.selected_effect_index = None
            return
        effects = self.current_effects()
        if row >= len(effects):
            self.selected_effect_index = None
            return
        self.selected_effect_index = row
        self.apply_effect_to_form(effects[row])

    def replace_selected_effect(self):
        if self.selected_effect_index is None:
            QMessageBox.warning(self, "No selection", "Select an effect to replace.")
            return
        effects = list(self.current_effects())
        i = self.selected_effect_index
        if i < 0 or i >= len(effects):
            return
        effects[i] = self.read_effect_from_form()
        self.refresh_effects_list(effects)
        self.effects_list.setCurrentRow(i)

    def remove_selected_effect(self):
        if self.selected_effect_index is None:
            return
        effects = list(self.current_effects())
        i = self.selected_effect_index
        if i < 0 or i >= len(effects):
            return
        del effects[i]
        self.refresh_effects_list(effects)
        self.selected_effect_index = None
        self.effects_list.setCurrentRow(-1)


def main():
    ap = argparse.ArgumentParser(description="Effect YAML Tool")
    ap.add_argument("--schema", required=True, help="Path to effect_schema.json")
    ap.add_argument("--yaml", required=True, help="Path to actions.yaml")
    args = ap.parse_args()

    app = QApplication([])
    w = App(args.schema, args.yaml)
    w.resize(1100, 700)
    w.show()
    sys.exit(app.exec())


if __name__ == "__main__":
    main()
