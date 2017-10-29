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
uniform float time;

void main(void)
{
	vec3 p = pos;

	p *= sin(time*8.0) + cos(p.x) + sin(p.y);
	vec3 col = vec3(0.5) + p * 0.5;
	col.b = sin(time) * 0.5+0.5;

	out_Colour = vec4(col,1.0);
}
