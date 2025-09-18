#version 330

in vec3 vertexPosition;
out vec3 vPosition;

uniform mat4 mvp;

void main() {
    vPosition = vertexPosition;
    gl_Position = mvp * vec4(vertexPosition, 1.0);
}
