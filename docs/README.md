# PlantUML Diagrams for LabyrinthRPG

This directory contains PlantUML diagrams that visualize the architecture and relationships between types in the LabyrinthRPG project.

## Diagram Files

1. **01_overall_architecture.puml** - Complete class diagram showing all major systems
2. **02_battle_system_detail.puml** - Detailed view of the battle system components
3. **03_inventory_equipment.puml** - Inventory and equipment management system
4. **04_dialogue_system.puml** - Dialogue node graph structure
5. **05_service_dependencies.puml** - Dependency injection and service registration
6. **06_complete_system.puml** - High-level component view of the entire system

## Installation

### Install PlantUML

#### Option 1: Using Java (Recommended)
```bash
# Install Java if not already installed
sudo apt update
sudo apt install default-jre

# Download PlantUML JAR
wget https://github.com/plantuml/plantuml/releases/download/v1.2024.0/plantuml-1.2024.0.jar -O plantuml.jar
```

# PlantUML Diagrams

Architecture diagrams for LabyrinthRPG showing relationships between types.

## Arch Linux Setup & Usage

```bash
# Install
sudo pacman -S plantuml graphviz

# Generate all diagrams as PNG
cd /home/river/Documents/Godot/LabyrinthRPG/docs/diagrams
plantuml *.puml

# Generate as SVG (vector format)
plantuml -tsvg *.puml

# Generate specific diagram
plantuml 01_overall_architecture.puml

# View diagrams
feh *.png
# or
eog *.png
# or open in browser
xdg-open 01_overall_architecture.png
