#version 330

in vec3 vPosition;
out vec4 fragColor;

uniform float uTime;
uniform vec3 uSunDir;

// Hash & noise helpers
float hash(vec3 p) {
    return fract(sin(dot(p, vec3(127.1,311.7,74.7))) * 43758.5453);
}

float noise(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    float a = hash(i);
    float b = hash(i + vec3(1,0,0));
    float c = hash(i + vec3(0,1,0));
    float d = hash(i + vec3(1,1,0));
    float e = hash(i + vec3(0,0,1));
    float f1 = hash(i + vec3(1,0,1));
    float g = hash(i + vec3(0,1,1));
    float h = hash(i + vec3(1,1,1));
    vec3 u = f*f*(3.0-2.0*f);
    return mix(mix(mix(a,b,u.x), mix(c,d,u.x), u.y),
               mix(mix(e,f1,u.x), mix(g,h,u.x), u.y), u.z);
}

float fbm(vec3 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i=0; i<5; i++) {
        v += a * noise(p);
        p *= 2.0;
        a *= 0.5;
    }
    return v;
}

void main() {
    vec3 dir = normalize(vPosition);

    // Cloud noise
    vec3 samplePos = dir * 2.0 + vec3(uTime*0.02, 0.0, uTime*0.01);
    float c = fbm(samplePos);
    c = smoothstep(0.55, 0.7, c);

    // Sky gradient
    float t = clamp(dir.y*0.5 + 0.5, 0.0, 1.0);
    vec3 skyTop = vec3(0.25, 0.45, 0.8);
    vec3 skyBot = vec3(0.7, 0.85, 0.95);
    vec3 skyCol = mix(skyBot, skyTop, t);

    // Sun disk
    float sunAngle = max(dot(dir, normalize(uSunDir)), 0.0);
    float sunDisk = smoothstep(0.9995, 1.0, sunAngle);  // sharp disk
    float sunGlow = smoothstep(0.98, 1.0, sunAngle);    // halo glow
    vec3 sunColor = vec3(1.0, 0.95, 0.8);

    // Clouds lit by sun
    float sunL = max(dot(dir, normalize(uSunDir)), 0.0);
    vec3 cloudCol = vec3(0.95,0.95,1.0)*(0.6 + sunL*1.2);

    // Base sky with sun
    vec3 col = skyCol;
    col = mix(col, sunColor, sunGlow);  // halo
    col = mix(col, sunColor*1.2, sunDisk); // bright core

    // Add clouds
    col = mix(col, cloudCol, c);

    fragColor = vec4(col, 1.0);
}
