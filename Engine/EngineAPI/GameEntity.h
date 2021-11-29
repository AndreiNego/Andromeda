#pragma once

#include "Components/ComponentsHeaders.h"
#include "TransformComponent.h"

namespace andromeda::game_entity {
	DEFINE_TYPED_ID(entity_id);
	class entity {
	public:
		constexpr explicit entity(entity_id id) : _id{ id } {}
		constexpr explicit entity() : _id{ id::invalid_id } {}
		constexpr entity_id get_id() const { return _id; }
		const bool is_valid() const { return id::is_valid(_id); }

		transform::component transform() const;
	private:
		entity_id _id;
	};
}