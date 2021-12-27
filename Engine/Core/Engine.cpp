
#if !defined(SHIPPING)
#include "Content/ContentLoader.h"
#include "Components/Script.h"
#include <thread>

bool engine_initialize()
{
	bool result{ andromeda::content::load_game() };
	return result;
}
void engine_update()
{
	andromeda::script::update(10.f);
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}
void engine_shutdown()
{
	andromeda::content::unload_game();
}
#endif //!defined(SHIPPING)