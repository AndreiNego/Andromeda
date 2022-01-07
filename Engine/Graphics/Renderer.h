#pragma once
#include "CommonHeaders.h"
#include "..\Platform\Window.h"

namespace andromeda::graphics {

	class surface
	{};

	struct render_surface
	{
		platform::window window{};
		surface surface{};
	};
	enum class graphics_platform :u32 {
		direct3d12 = 0, 
		//vulkan = 1,
		//open_gl = 2,
	};
	bool initialize(graphics_platform platform);
	void shutdown();
}