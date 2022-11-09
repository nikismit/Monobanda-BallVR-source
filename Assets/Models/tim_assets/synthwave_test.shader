// https://stackoverflow.com/questions/12964279/whats-the-origin-of-this-glsl-rand-one-liner
float rand(vec2 co)
{
    return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = fragCoord / iResolution.xy;

#if 1 // distortions
    float t1 = rand(vec2(floor(iTime), 1.));
    float t2 = rand(vec2(floor(iTime), 2.));
    float t3 = rand(vec2(floor(iTime), 3.));
    float t4 = rand(vec2(floor(iTime), 4.));

    if (mod(iTime + t1, 2.) < .1)
    	uv.y += .005 * texture(iChannel2, uv).y;
    if (mod(iTime + t2, 3.) < .1 && uv.y < mod(iTime, 1.))
    	uv.x -= .01 * texture(iChannel2, uv + iTime).x;
    if (mod(iTime + t3, 5.) < .1)
        uv.x += .5 * sin(20. * radians(360.) * uv.y + iTime);
    if (mod(iTime + t4, 7.) < .06)
        uv.x += .007 * sin(20. * radians(360.) * uv.y + 3. * radians(360.) * iTime);
#endif

    fragColor = max(texture(iChannel0, uv), texture(iChannel1, uv));
}
