@tool
extends GraphNode

func _on_resize_request(new_minsize):
	self.set_size(new_minsize)

func _on_close_request():
	queue_free()

func _on_button_pressed():
	print(position_offset)
