#ifndef _skul_
#define _skul_
#include <rectangl.cpp>
#include <metlistp.cpp>
class Skull
	{
	private:
	Rectangle rec;
	Points position;
	float height;
        float width;
	public:
	Skull(Points position1,float height1,float width1);
	Skull(const Skull &b);
	List<Points> GetPoints();
	float GetHeight();
	}
;
#endif