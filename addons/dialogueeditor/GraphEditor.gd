@tool
extends GraphEdit

var context_menu
var buttons
var _file_dialog

# Called when the node enters the scene tree for the first time.
func _ready():
	get_node("../FileDialog").file_selected.connect(self.save_dialogue)
	_file_dialog = get_node("../FileDialog")
	context_menu = get_node("../ContextMenu")
	buttons = get_node("../ContextMenu/Buttons").get_children()
	for button in buttons:
		button.connect("pressed", hide_context_menu)
	set_right_disconnects(true)

func _on_connection_request(from_node, from_port, to_node, to_port):
	connect_node(from_node, from_port, to_node, to_port)

func _on_disconnection_request(from_node, from_port, to_node, to_port):
	disconnect_node(from_node, from_port, to_node, to_port)

func _on_popup_request(position):
	# spawn popup menu with the three buttons
	var cm = get_node("../ContextMenu")
	context_menu.position = position
	context_menu.visible = true

func _on_gui_input(event):
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		hide_context_menu()

func hide_context_menu():
	context_menu.visible = false

func _on_save_pressed():
	get_node(^"../FileDialog").popup_centered()
	print(get_connection_list())
	
func save_dialogue(path):
	print("saved at " + path)
	


func _on_debug_dialogue_pressed():
	save_dialogue("res://data/dialogues")
