﻿//|vert
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
uniform sampler2D tex2;

void main(void)
{
	vec4 t1 = texture(tex1,(pos.xy*vec2(0.5,-0.5)+0.5).xy).rgba;
	vec4 t2 = texture(tex2,(pos.xy*vec2(0.5,-0.5)+0.5).xy).rgba;

	vec3 col = vec3(0.5) + pos * 0.5;
	col.b = 0.6;

	col.r += t1.r * 0.2;
	col.g += t2.g * 0.2;
	
	out_Colour = vec4(col,1.0);
}
