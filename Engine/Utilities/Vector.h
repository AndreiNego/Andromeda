#pragma once
#include "CommonHeaders.h"

namespace andromeda::utl {


	//A vector class similar to std::vector with basic funtionality.
	//The user can specify in the template argument whether they want the element distructor to be called when being removed or while clearing the vector.
	template<typename T, bool destruct = true>
	class vector
	{
	public:
		//Default constructor. Doesn't allocate memory.
		vector() = default;

		//Constructor resizes the vector and initializes count items.'
		constexpr explicit vector(u64 count)
		{
			resize(count);
		}

		constexpr explicit vector(u64 count, const T& value)
		{
			resize(count, value);
		}
		// copy-constructor. construct by copying another vector. the items in the copied vector must be copyable.
		constexpr vector(const vector& o)
		{
			*this = o;
		}
		//Move c-tor. Construct by moving another vector. The original vector will be empty after the move.
		constexpr vector(const vector&& o)
			: _capacity{o._capacity}, _size{o._size}, _data{o._data}
		{
			o.reset();
		}
		// copy assignment operator. clears this vector and copies items from another vector. the items must be copyable.
		constexpr vector& operator=(const vector& o)
		{
			assert(this != std::addressof(o));
			if (this != std::addressof(o))
			{
				clear();
				reserve(o.size);
				for (auto& item : o)
				{
					emplace_back(item);
				}
				assert(_size == o._size);
			}
			return *this;
		}
		//Move-assigment operator. Frees all resources in this vector and moves the other vector into this one
		constexpr vector& operator=(vector&& o)
		{
			assert(this != std::addressof(o));
			if (this != std::addressof(o))
			{
				destroy();
				move(o);
			}
			return *this;
		}
		//destruct the vector and its items as specified in template argument
		~vector() { destroy(); }


		//resizes the vector and initializez new items with their default value.
		constexpr void resize(u64 new_size)
		{
			static_assert(std::is_default_constructible_v<T>), "Type must be default-constructible.");
			
			if (new_size > _size)
			{
				reserve(new_size);
				while (_size < new_size)
				{
					emplace_back();
				}
			}
			else if (new_size < _ size)
			{
				if constexpr (destruct)
				{
					destruct_range(new_size, size);
				}
			}
			//do nothing if new_size == size.
			assert(new_size == _size);
		}

		//clears the vector and destructs items as specified in template argument.
		constexpr void clear()
		{
			if constexpr (destruct)
			{
				destruct_range(0, _size);
			}
			_size = 0;
		}

	private:
		constexpr void move(vector& o)
		{
			_capacity = o._capacity;
			_size = o._size;
			_data = o._data;
			o.reset();
		}

		constexpr void reset()
		{
			_capacity = 0;
			_size = 0;
			_data = nullptr;
		}

		constexpr void destruct_range(u64 first, u64 last)
		{
			assert(destruct);
			assert(first <= _size && last <= _size && first <= last);
			if (_data)
			{
				for (; first != last; ++first)
				{
					_data[first].~T();
				}
			}
		}

		constexpr void destroy()
		{
			assert([&] {return _capacity ? _data != nullptr : _data == nullptr}());
			clear();
			_capacity = 0;
			if (_data) free(_data);
			_data = nullptr;
		}

		u64 _capacity{ 0 };
		u64 _size{ 0 };
		T* _data{ nullptr };
	};
}