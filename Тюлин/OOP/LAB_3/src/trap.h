#ifndef my_trap
#define my_trap

#include "point.h"

class Trapezium
{
private:
	Point lu;
	Point ru;
	Point ld;
	Point rd;
public:
	Trapezium(Point pos, double height, double down, double top);
	void MoveBy(Point dxdy);
	Point GetLeftUp() const;
	Point GetRightUp() const;
	Point GetLeftDown() const;
	Point GetRightDown() const;
};

#endif