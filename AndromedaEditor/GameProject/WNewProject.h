#pragma once
#include <Windows.h>
#include <iostream>
#include <string>
namespace Andromeda {
	class WNewProject
	{
	private:
		std::string defaultName = "NewProject";
		LPCSTR _name = defaultName.c_str();
	public:
		void setName(LPCSTR s)
		{
			_name = s;

		};
		LPCSTR getName() {
			return _name;
		}
	};
}

