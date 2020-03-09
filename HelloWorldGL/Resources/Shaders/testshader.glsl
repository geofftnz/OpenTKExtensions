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
uniform sampler2D tex_new;

void main(void)
{
	vec2 p = (pos.xy*vec2(0.5,-0.5)+0.5);
	vec4 t1 = texture(tex1,p).rgba;
	vec4 t2 = vec4(0.0);
	t2 = texture(tex_new,p).rgba;

	vec3 col = vec3(0.5) + pos * 0.5;

	col = mix(col,t1.rgb,t1.a);
	col = mix(col,t2.rgb,0.5);

	out_Colour = vec4(col,1.0);
}
