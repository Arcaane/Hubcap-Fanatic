// Make a float3 from the x, y, z components of a float4
void MakeVec3XYZFromVec4_float(float4 vec4, out float3 vec2)
{
    vec2 = float3(vec4.x, vec4.y, vec4.z);
}

// Make a float3 from the x, y, w components of a float4
void MakeVec3XYWFromVec4_float(float4 vec4, out float3 vec3)
{
    vec3 = float3(vec4.x, vec4.y, vec4.w);
}

// Make a float2 from the x and y components of a float4
void MakeVec2XYFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.x, vec4.y);
}
// Make a float2 from the x and z components of a float4
void MakeVec2XZFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.x, vec4.z);
}

// Make a float2 from the x and w components of a float4
void MakeVec2XWFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.x, vec4.w);
}

// Make a float2 from the y and z components of a float4
void MakeVec2YZFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.y, vec4.z);
}

// Make a float2 from the y and w components of a float4
void MakeVec2YWFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.y, vec4.w);
}

// Make a float2 from the z and w components of a float4
void MakeVec2ZWFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.z, vec4.w);
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// float make vector 4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Make a float4 from the x, y, z, w components of a float4
void MakeVec4XYZWFromVec4_float(float4 vec4, out float4 vec4Out)
{
    vec4Out = float4(vec4.x, vec4.y, vec4.z, vec4.w);
}

// Make a float4 from the x, y, z, w components of a float3
void MakeVec4XYZWFromVec3_float(float3 vec3, out float4 vec4Out)
{
    vec4Out = float4(vec3.x, vec3.y, vec3.z, 1.0f);
}

// Make a float4 from the x, y, z, w components of a float2
void MakeVec4XYZWFromVec2_float(float2 vec2, out float4 vec4Out)
{
    vec4Out = float4(vec2.x, vec2.y, 0.0f, 1.0f);
}

// Make a float4 from the x, y, z, w components of a float1
void MakeVec4XYZWFromFloat_float(float value, out float4 vec4Out)
{
    vec4Out = float4(value, value, value, value);
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// int make vector 4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Make a float4 from the x, y, z, w components of a int4
void MakeVec4XYZWFromVec4_int(int4 vec4, out float4 vec4Out)
{
    vec4Out = float4(vec4.x, vec4.y, vec4.z, vec4.w);
}

// Make a float4 from the x, y, z, w components of a int3
void MakeVec4XYZWFromVec3_int(int3 vec3, out float4 vec4Out)
{
    vec4Out = float4(vec3.x, vec3.y, vec3.z, 1.0f);
}

// Make a float4 from the x, y, z, w components of a int2
void MakeVec4XYZWFromVec2_int(int2 vec2, out float4 vec4Out)
{
    vec4Out = float4(vec2.x, vec2.y, 0.0f, 1.0f);
}

// Make a float4 from the x, y, z, w components of a int1
void MakeVec4XYZWFromInt_int(int value, out float4 vec4Out)
{
    vec4Out = float4(value, value, value, value);
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// uint make vector 4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Make a float4 from the x, y, z, w components of a uint4
void MakeVec4XYZWFromVec4_uint(uint4 vec4, out float4 vec4Out)
{
    vec4Out = float4(vec4.x, vec4.y, vec4.z, vec4.w);
}

// Make a float4 from the x, y, z, w components of a uint3
void MakeVec4XYZWFromVec3_uint(uint3 vec3, out float4 vec4Out)
{
    vec4Out = float4(vec3.x, vec3.y, vec3.z, 1.0f);
}

// Make a float4 from the x, y, z, w components of a uint2
void MakeVec4XYZWFromVec2_uint(uint2 vec2, out float4 vec4Out)
{
    vec4Out = float4(vec2.x, vec2.y, 0.0f, 1.0f);
}

// Make a float4 from the x, y, z, w components of a uint1
void MakeVec4XYZWFromUint_uint(uint value, out float4 vec4Out)
{
    vec4Out = float4(value, value, value, value);
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// LIGHTING
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
    #if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
    #else
    Light mainLight = GetMainLight();
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;

    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    half shadowStrength = GetMainLightShadowStrength();
    ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
    #endif
}

void DirectSpecular_float(float3 Specular, float Smoothness, float3 Direction, float3 Color, float3 WorldNormal, float3 WorldView, out float3 Out)
{ 
    #if SHADERGRAPH_PREVIEW
    Out = 0;
    #else
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = SafeNormalize(WorldView);
    Out = LightingSpecular(Color, Direction, WorldNormal, WorldView, float4(Specular, 0), Smoothness);
    #endif
}

void GetMainLightDirection_float(out float3 MainLightDirection, out float MainLightIntensity)
{
    #ifdef SHADERGRAPH_PREVIEW
    MainLightDirection = float3(0.5,0.5,0);
    MainLightIntensity = 1.0f;
    #else
    MainLightDirection = GetMainLight().direction;
    MainLightIntensity = length(GetMainLight().color);
    #endif
	
}

void Refract_float(float3 View, float3 Normal, float IOR, out float3 Out)
{
    Out = refract(View, Normal, IOR);
}

