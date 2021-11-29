#pragma once

#include "CommonHeaders.h"
#include <DirectXMath.h>
namespace andromeda::math {
	constexpr float pi = 3.14159265358979323f;
	constexpr float epsilon = 1e-5f;
	
	
	
	class Vector3 {
	protected: double X, Y, Z;

	public:

		Vector3(double _x, double _y, double _z) : X(_x), Y(_y), Z(_z) {

			double vector3[3]{};
			vector3[0] = _x;
			vector3[1] = _y;
			vector3[2] = _z;

		};

	};
	class Quaternion {
	protected: double X, Y, Z, W;

	public:
		Quaternion(double _x, double _y, double _z, double _w) : X(_x), Y(_y), Z(_z), W(_w) {
			double quaternion[4]{};
			quaternion[0] = _x;
			quaternion[1] = _y;
			quaternion[2] = _z;
			quaternion[3] = _w;
		};

	};
}