//|vert
#version 450
precision highp float;
layout (location = 0) in vec3 vertex;
layout (location = 0) out vec3 pos;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;

void main() 
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex,1.0);
	pos = gl_Position.xyz;
}

//|frag
#version 450
precision highp float;
layout (location = 0) in vec3 pos;
layout (location = 0) out vec4 out_Colour;
uniform sampler2D tex1;

void main(void)
{
	vec4 t = texture(tex1,(pos.xy*vec2(0.5,-0.5)+0.5).xy).rgba;

	vec3 col = vec3(0.5) + pos * 0.5;
	col.b = 0.6;

	t.rgb += col * 0.1;

	//t.a = mod(pos.x*32.0,1.0) * mod(pos.y*32.0,1.0);

	//col = t;

	//t.rgb = pow(t.rgb,vec3(1.0 / 2.2));

	out_Colour = t;
}
