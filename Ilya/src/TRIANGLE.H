#include <metpoint.cpp>
class Triangle
	{
	private:
	Points Left;
	Points Right;
	float height;
	public:
	Triangle(const Points Left1,const Points Right1,float height1);
	Points GetLeft() const;
	Points GetRight() const;
	float GetHeight() const;
	void MoveBy(float dx,float dy);
	}
;