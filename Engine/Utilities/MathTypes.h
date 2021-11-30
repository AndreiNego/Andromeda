#pragma once
//#include "CommonHeaders.h"
#if defined (_WIN64)
#include <DirectXMath.h>
#endif
namespace andromeda::math {
	constexpr float pi = 3.14159265358979323f;
	constexpr float epsilon = 1e-5f;

	using Vector2 = DirectX::XMFLOAT2;
	using Vector3 = DirectX::XMFLOAT3;
	using Vector4 = DirectX::XMFLOAT4;
	

}