#pragma once
#include "CommonHeaders.h"

#if !defined(SHIPPING)
namespace andromeda::content {
	bool load_game();
	void unload_game();
}
#endif //!defined shipping