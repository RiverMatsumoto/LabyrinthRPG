@tool
extends BoxContainer

func _on_create_text_node_pressed():
	var text_node = load("res://addons/dialogueeditor/TextNode.tscn").instantiate()
	text_node.position_offset = get_parent().position - Vector2(150, 60)
	print(self.position)

	print(text_node.position_offset)
	get_node("../../GraphEdit").add_child(text_node)

func _on_add_choice_node_pressed():
	pass # Replace with function body.
