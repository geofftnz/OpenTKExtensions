//|vert
#version 450
precision highp float;
layout (location = 0) in vec2 vertex;
layout (location = 0) out vec3 pos;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;

void main() 
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.x,0.0,vertex.y,1.0);
	pos = gl_Position.xyz;
}

//|frag
#version 450
precision highp float;
layout (location = 0) in vec3 pos;
layout (location = 0) out vec4 out_Colour;

void main(void)
{
	vec3 col = vec3(0.1) + vec3(pos);
	out_Colour = vec4(col,1.0);
}
