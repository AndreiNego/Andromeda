#pragma once
#include "../Components/ComponentsHeaders.h"

namespace andromeda::script {

	DEFINE_TYPED_ID(script_id);

	class component final {
	public:
		constexpr explicit component(script_id id) : _id{ id } {}
		constexpr component() : _id{ id::invalid_id } {}
		constexpr script_id get_id() const { return _id; }
		const bool is_valid() const { return id::is_valid(_id); }

		
	private:
		script_id _id;
	};
}