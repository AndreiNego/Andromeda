#pragma once
#include "ComponentsHeaders.h"

namespace andromeda::transform {


	
	struct init_info
	{
		math::Quaternion rotation =  math::Quaternion(0, 0, 0, 1);
		math::Vector3 position = math::Vector3(0, 0, 0);
		math::Vector3 scale = math::Vector3(0, 0, 0);
	//	double position[3]{};
	//	double rotation[4]{1,1,1,1};
	//	double scale[3]{ 1,1,1 };
	};
	component create_transform(const init_info& info, game_entity::entity entity);
	void remove_transform(component c);
}