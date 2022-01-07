#pragma once
#include "CommonHeaders.h"
#include "Renderer.h"

namespace andromeda::graphics {
	struct platform_interface
	{
		bool(*initialize)(void);
		void(*shutdown)(void);
		void(*render)(void);
	};
}