shader_type spatial;

render_mode unshaded;

uniform vec4 outline_color : source_color;
uniform float outline_width : hint_range(0.0, 5.0) = 1.0f;
uniform float fade_start : hint_range(0.0, 100.0) = 10.0f;
uniform float fade_end : hint_range(0.0, 100.0) = 30.0f;
uniform bool isHovered = false;

void vertex() {
	// Convert the vertex position to clip space
	vec4 clip_position = PROJECTION_MATRIX * (MODELVIEW_MATRIX * vec4(VERTEX, 1.0));

	// Transform the normal into clip space
	vec3 clip_normal = mat3(PROJECTION_MATRIX * MODELVIEW_MATRIX) * NORMAL;

	// Calculate offset for outline expansion
	vec2 offset = normalize(clip_normal.xy) * outline_width * clip_position.w / VIEWPORT_SIZE;

	// Apply the offset to create the outline effect
	clip_position.xy += offset * 2.0;

	// Set final position
	POSITION = clip_position;
}

void fragment() {
	if (!isHovered) discard;
	float dist = length(VERTEX);
	float fade = clamp(1.0 - (dist - fade_start) / (fade_end - fade_start), 0.0, 1.0);
	ALBEDO = outline_color.rgb;
	//ALPHA = outline_color.a * fade;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
