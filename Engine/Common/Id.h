#pragma once
#include "CommonHeaders.h"

namespace andromeda::id {
	using id_type = u64;

	constexpr u64 generation_bits{ 12 };
	constexpr u64 index_bits{ sizeof(id_type) * 10 -generation_bits };
	constexpr id_type index_mask{ (id_type{1} << index_bits) - 1 };
	constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
	constexpr id_type id_mask{ id_type{-1} };
}