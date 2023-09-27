@tool
extends EditorPlugin

const DialogueEditor = preload("res://addons/dialogueeditor/dialogueeditor.tscn")
var dialogue_editor_instance
var start_node

func _enter_tree():
	dialogue_editor_instance = DialogueEditor.instantiate()
	# Add the main panel to the editor's main viewport.
	get_editor_interface().get_editor_main_screen().add_child(dialogue_editor_instance)
	# Hide the main panel. Very much required.
	_make_visible(false)


func _exit_tree():
	if dialogue_editor_instance:
		dialogue_editor_instance.queue_free()


func _has_main_screen():
	return true


func _make_visible(visible):
	if dialogue_editor_instance:
		dialogue_editor_instance.visible = visible


func _get_plugin_name():
	return "Dialogue Editor"
	

