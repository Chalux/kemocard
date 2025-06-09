shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = vec4(COLOR.rgb,min(COLOR.a,1.0-pow(UV.y,1)));
}
