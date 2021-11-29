#pragma once
#include "ComponentsHeaders.h"

namespace andromeda {

#define INIT_INFO(component) namespace transform { struct init_info; }
	INIT_INFO(component);
#undef INIT_INFO

	namespace game_entity {
		struct entity_info {
			transform::init_info* transform{ nullptr };
		};

		entity create_game_entity(const entity_info& info);
		void remove_game_entity(entity e);
		bool is_alive(entity e);
	}
}