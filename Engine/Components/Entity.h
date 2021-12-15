#pragma once
#include "ComponentsHeaders.h"

namespace andromeda {

#define INIT_INFO(component)
	INIT_INFO(transform)  namespace transform { struct init_info; }
	INIT_INFO(script) namespace script { struct init_info; }
#undef INIT_INFO

	namespace game_entity {
		struct entity_info {
			transform::init_info* transform{ nullptr };
			script::init_info* script{ nullptr };
		};

		entity create(entity_info info);
		void remove(entity_id id);
		bool is_alive(entity_id id);
	}
}