shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

vec4 newColor(float oldTone, float heat) {
	return (vec4( cos(heat * PI)/2.0 + 0.5 + heat/10.0 -.05, cos(heat * PI/2.0) - 0.1, heat, 1.0) * oldTone);
}

void fragment() {
	// Called for every pixel the material is visible on.
	float RedTones = COLOR.r;
	float BlueTones = COLOR.b;
	float Alpha = COLOR.a;
	COLOR = vec4(newColor(RedTones, mod(TIME/3600.0, 1.0)) + newColor(BlueTones, mod(TIME/3600.0, 1.0)));
	COLOR.a = Alpha;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
