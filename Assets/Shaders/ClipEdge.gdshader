shader_type canvas_item;
const float threshold = 0.01f;

void vertex() {
	
}

void fragment() {
	bool is_edge =
        (UV.x < threshold || UV.x > (1.f-threshold)) ||  // 左右边缘
        (UV.y < threshold || UV.y > (1.f-threshold));   // 上下边缘
	if (is_edge) {
        COLOR = texture(TEXTURE, UV);
    } else {
        COLOR = vec4(0.0, 0.0, 0.0, 0.0); // 透明
    }
}
