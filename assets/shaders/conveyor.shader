
HEADER
{
	Description = "";
}

FEATURES
{
	#include "common/features.hlsl"
}

MODES
{
	VrForward();
	Depth(); 
	ToolsVis( S_MODE_TOOLS_VIS );
	ToolsWireframe( "vr_tools_wireframe.shader" );
	ToolsShadingComplexity( "tools_shading_complexity.shader" );
}

COMMON
{
	#ifndef S_ALPHA_TEST
	#define S_ALPHA_TEST 0
	#endif
	#ifndef S_TRANSLUCENT
	#define S_TRANSLUCENT 0
	#endif
	
	#include "common/shared.hlsl"
	#include "procedural.hlsl"

	#define S_UV2 1
	#define CUSTOM_MATERIAL_INPUTS
}

struct VertexInput
{
	#include "common/vertexinput.hlsl"
	float4 vColor : COLOR0 < Semantic( Color ); >;
};

struct PixelInput
{
	#include "common/pixelinput.hlsl"
	float3 vPositionOs : TEXCOORD14;
	float3 vNormalOs : TEXCOORD15;
	float4 vTangentUOs_flTangentVSign : TANGENT	< Semantic( TangentU_SignV ); >;
	float4 vColor : COLOR0;
};

VS
{
	#include "common/vertex.hlsl"

	PixelInput MainVs( VertexInput v )
	{
		PixelInput i = ProcessVertex( v );
		i.vPositionOs = v.vPositionOs.xyz;
		i.vColor = v.vColor;

		VS_DecodeObjectSpaceNormalAndTangent( v, i.vNormalOs, i.vTangentUOs_flTangentVSign );

		return FinalizeVertex( i );
	}
}

PS
{
	#include "common/pixel.hlsl"
	
	SamplerState g_sSampler0 < Filter( ANISO ); AddressU( WRAP ); AddressV( WRAP ); >;
	CreateInputTexture2D( Color, Srgb, 8, "None", "_color", "Color,0/,0/0", Default4( 1.00, 1.00, 1.00, 1.00 ) );
	CreateInputTexture2D( Normal, Srgb, 8, "None", "_normal", "Normal,1/,0/0", Default4( 1.00, 1.00, 1.00, 1.00 ) );
	CreateInputTexture2D( Rough, Srgb, 8, "None", "_rough", "Rough,2/,0/0", Default4( 1.00, 1.00, 1.00, 1.00 ) );
	CreateInputTexture2D( Metalness, Srgb, 8, "None", "_metal", "Metalness,3/,0/0", Default4( 1.00, 1.00, 1.00, 1.00 ) );
	CreateInputTexture2D( AmbientOcclusion, Srgb, 8, "None", "_ao", "Ambient Occlusion,4/,0/0", Default4( 1.00, 1.00, 1.00, 1.00 ) );
	Texture2D g_tColor < Channel( RGBA, Box( Color ), Srgb ); OutputFormat( DXT5 ); SrgbRead( True ); >;
	Texture2D g_tNormal < Channel( RGBA, Box( Normal ), Srgb ); OutputFormat( DXT5 ); SrgbRead( True ); >;
	Texture2D g_tRough < Channel( RGBA, Box( Rough ), Srgb ); OutputFormat( DXT5 ); SrgbRead( True ); >;
	Texture2D g_tMetalness < Channel( RGBA, Box( Metalness ), Srgb ); OutputFormat( DXT5 ); SrgbRead( True ); >;
	Texture2D g_tAmbientOcclusion < Channel( RGBA, Box( AmbientOcclusion ), Srgb ); OutputFormat( DXT5 ); SrgbRead( True ); >;
	float g_flSpeed < UiGroup( "Speed,5/,0/0" ); Default1( 1 ); Range1( 0, 128 ); >;
	
	float4 MainPs( PixelInput i ) : SV_Target0
	{
		Material m = Material::Init();
		m.Albedo = float3( 1, 1, 1 );
		m.Normal = float3( 0, 0, 1 );
		m.Roughness = 1;
		m.Metalness = 0;
		m.AmbientOcclusion = 1;
		m.TintMask = 1;
		m.Opacity = 1;
		m.Emission = float3( 0, 0, 0 );
		m.Transmission = 0;
		
		float2 l_0 = i.vTextureCoords.xy * float2( 1, 1 );
		float l_1 = l_0.x;
		float l_2 = round( l_1 );
		float l_3 = g_flSpeed;
		float l_4 = g_flTime * l_3;
		float l_5 = frac( l_4 );
		float l_6 = l_2 * l_5;
		float4 l_7 = float4( 0, l_6, 0, 0 );
		float2 l_8 = TileAndOffsetUv( l_0, float2( 1, 1 ), l_7.xy );
		float4 l_9 = Tex2DS( g_tColor, g_sSampler0, l_8 );
		float4 l_10 = Tex2DS( g_tNormal, g_sSampler0, l_8 );
		float4 l_11 = Tex2DS( g_tRough, g_sSampler0, l_8 );
		float4 l_12 = Tex2DS( g_tMetalness, g_sSampler0, l_8 );
		float4 l_13 = Tex2DS( g_tAmbientOcclusion, g_sSampler0, l_8 );
		
		m.Albedo = l_9.xyz;
		m.Opacity = 1;
		m.Normal = l_10.xyz;
		m.Roughness = l_11.x;
		m.Metalness = l_12.x;
		m.AmbientOcclusion = l_13.x;
		
		m.AmbientOcclusion = saturate( m.AmbientOcclusion );
		m.Roughness = saturate( m.Roughness );
		m.Metalness = saturate( m.Metalness );
		m.Opacity = saturate( m.Opacity );

		// Result node takes normal as tangent space, convert it to world space now
		m.Normal = TransformNormal( m.Normal, i.vNormalWs, i.vTangentUWs, i.vTangentVWs );

		// for some toolvis shit
		m.WorldTangentU = i.vTangentUWs;
		m.WorldTangentV = i.vTangentVWs;
        m.TextureCoords = i.vTextureCoords.xy;
		
		return ShadingModelStandard::Shade( i, m );
	}
}
