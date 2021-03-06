#ifndef _qwskull_
#define _qwskull_
#include <skull.h>
#include <iostream.h>
Skull::Skull(Points position1,float height1,float width1)
:rec(position1,
position1.MoveBy(width1,0),
position1.MoveBy(0,height1*(-1)),
position1.MoveBy(width1,height1*(-1))),
width(width1), height(height1)
{
#ifdef TALKY
cout<<"skull created"<<endl;
#endif
}

Skull::Skull(const Skull &b)
:rec(b.rec)
{
#ifdef TALKY
cout<<"Skull created"<<endl;
#endif
}

float Skull::GetHeight()
{
return height;
}

float Skull::GetWidth()
{
return width;
}

List<Points> Skull::GetPoints() const
{
List<Points> list;
list.Add(rec.GetTopLeft());
list.Add(rec.GetTopRight());
list.Add(rec.GetBottomLeft());
list.Add(rec.GetBottomRight());
return list;
}

void Skull::MoveBy(float dx, float dy) 
{
	rec.MoveBy(dx, dy);
}
#endif