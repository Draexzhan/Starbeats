shader_type canvas_item;
uniform float heats;

void vertex() {
	// Called for every vertex the material is visible on.
}

vec4 newColor(float oldTone, float heat) {
	return (vec4( cos(heat * PI)/7.0 + 0.5 + heat/4.0, sin(heat * PI/3.0) * 0.9 + 0.2, heat, 1.0));
}

void fragment() {
	// Called for every pixel the material is visible on.
	float RedTones = COLOR.r;
	float BlueTones = COLOR.b;
	float Alpha = COLOR.a;
	COLOR = vec4(newColor(RedTones, heats) + newColor(BlueTones, heats));
	COLOR.a = Alpha;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
