#version 430

out vec4 FragColor;
in vec3 glPosition;

#define EPSILON 0.001
#define BIG 100000.0

const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;

struct SCamera 
{
    vec3 Position;
    vec3 View;
    vec3 Up;
    vec3 Side;
    vec2 Scale;
};

struct SSphere
{
    vec3 Center;
    float Radius;
};

struct STriangle{
	vec3 v1;
	vec3 v2;
	vec3 v3;
};

struct SRay 
{
    vec3 Orig;
    vec3 Dir;
};

struct SObject
{
    int Type;
    int GeomId;
    int MatId;
};

struct SLight
{
    vec3 Pos;
};

struct SMaterial
{
    vec3 Color;
    vec4 LightCoeffs;

    float Reflection;
    float Refraction;

    int Type;
};

SRay GenerateRay(SCamera uCam)
{
    vec2 coords = glPosition.xy * uCam.Scale;
    vec3 dir = uCam.View + uCam.Side * coords.x + uCam.Up * coords.y;
    return SRay(uCam.Position, normalize(dir));
}

SCamera uCamera;

uniform vec3 cameraPos;
uniform vec3 cameraView;
uniform vec3 cameraUp;
uniform vec3 cameraSide;
uniform vec2 cameraScale;

SCamera GetCamera()
{
    SCamera camera;
    camera.Position = cameraPos;
    camera.View = cameraView;
    camera.Up = cameraUp;
    camera.Side = cameraSide;
    camera.Scale = cameraScale;

    return camera;
}

SSphere spheres[3];
STriangle triangles[2];
SObject objects[5];
int objectsLength = 5;

SLight light;
SMaterial materials[3];

void InitObjects()
{
    spheres[0].Center = vec3(-1, -1, -2);
    spheres[0].Radius = 2.0;

    spheres[1].Center = vec3(2, 1, 2);
    spheres[1].Radius = 1.0;

    spheres[2].Center = vec3(0, 0, 0);
    spheres[2].Radius = 1.0;

    triangles[0].v1 = vec3(-5,-4,-5);
    triangles[0].v2 = vec3(-5,-4, 5);
    triangles[0].v3 = vec3(5,-4,5);

    triangles[1].v1 = vec3(5,-4,-5);
    triangles[1].v2 = vec3(-5,-4, -5);
    triangles[1].v3 = vec3(5,-4,5);

    objects[0].GeomId = 0;
    objects[0].MatId = 0;
    objects[0].Type = 0;

    objects[1].GeomId = 1;
    objects[1].MatId = 1;
    objects[1].Type = 0;

    objects[2].GeomId = 2;
    objects[2].MatId = 1;
    objects[2].Type = 0;

    objects[3].GeomId = 0;
    objects[3].MatId = 2;
    objects[3].Type = 1;

    objects[4].GeomId = 1;
    objects[4].MatId = 2;
    objects[4].Type = 1;


    light.Pos = vec3(0.0, 2.0, -4.0f);


    vec4 lightCoefs = vec4(0.7,0.9,0.0,512.0);
    materials[0].Color = vec3(0.0, 1.0, 0.0);
    materials[0].LightCoeffs = vec4(lightCoefs);
    materials[0].Reflection = 0.5;
    materials[0].Refraction = 1.0;
    materials[0].Type = DIFFUSE;

    materials[1].Color = vec3(0.2, 0.0, 1.0);
    materials[1].LightCoeffs = vec4(lightCoefs);
    materials[1].Reflection = 0.5;
    materials[1].Refraction = 1.0;
    materials[1].Type = DIFFUSE;

    materials[2].Color = vec3(1, 1, 1);
    materials[2].LightCoeffs = vec4(lightCoefs);
    materials[2].Reflection = 0.5;
    materials[2].Refraction = 1.0;
    materials[2].Type = DIFFUSE;

}

bool IsIntersectSphere(SRay r,SSphere s, out float t)
{
    r.Orig -= s.Center;
    float A = dot(r.Dir, r.Dir);
	float B = dot(r.Dir, r.Orig);
	float C = dot(r.Orig, r.Orig) - s.Radius * s.Radius;
	float D = B * B - A * C;
	if(D > 0.0){
		D = sqrt(D);
		float t1 = (-B - D) / A;
		float t2 = (-B + D) / A;
		if(t1 < 0 && t2 < 0){
			return false;
		}
		if(min(t1, t2) < 0){
			t = max(t1, t2);
			return true;
		}
		t = min(t1, t2);
		return true;
	}
	return false;
}

bool IsIntersectTriangle(SRay r, STriangle s, out float time){
	time = -1;

    vec3 v1 = s.v1;
    vec3 v2 = s.v2;
    vec3 v3 = s.v3;

	vec3 A = v2 - v1;
	vec3 B = v3 - v1;
	vec3 N = cross(A, B);
	float NdotRayDiraction = dot(N, r.Dir);
	if(abs(NdotRayDiraction) < EPSILON){
		return false;
	}
	float d = dot(N, v1);
	float t = -(dot(N, r.Orig) - d) / NdotRayDiraction;
	if(t < 0){
		return false;
	}
	vec3 P = r.Orig + t * r.Dir;
	vec3 C;
	vec3 edge1 = v2 - v1;
	vec3 VP1 = P - v1;
	C = cross(edge1, VP1);
	if(dot(N, C) < 0){
		return false;
	}
	vec3 edge2 = v3 - v2;
	vec3 VP2 = P - v2;
	C = cross(edge2, VP2);
	if(dot(N, C) < 0){
		return false;
	}
	vec3 edge3 = v1 - v3;
	vec3 VP3 = P - v3;
	C = cross(edge3, VP3);
	if(dot(N, C) < 0){
		return false;
	}
	time = t;
	return true;
}

bool RayTrace(SRay r, float final, out int id, out vec3 norm, out vec3 point)
{
    bool result = false;
    float time = final;
    float test = 0;

    for(int i = 0; i < objectsLength; ++i)
    {
        if(objects[i].Type == 0)
        {
            SSphere s = spheres[objects[i].GeomId];
            if(IsIntersectSphere(r,s,test) && test < time)
            {
                time = test;
                id = i;

                point = r.Orig + r.Dir * test;
                norm = normalize(point - s.Center);

                result = true;
            }
        }
         if(objects[i].Type == 1)
        {
            STriangle tr = triangles[objects[i].GeomId];
            if(IsIntersectTriangle(r,tr,test) && test < time)
            {
                time = test;
                id = i;

                point = r.Orig + r.Dir * test;
                norm = normalize(cross(tr.v1 - tr.v2, tr.v3 - tr.v2));

                result = true;
            }
        }
    }

    return result;
}

vec3 RayTraceColor(SRay r)
{
    vec3 tmp_v;
    int tmp_i;

    vec3 color = vec3(0.9,0.9,0.9);

    float time = -1;
    int oid = -1;
    vec3 norm;
    vec3 point;

    if(RayTrace(r,BIG,oid,norm,point))
    {
        SMaterial m = materials[objects[oid].MatId];
        color = vec3(m.Color);

        float shadow = 1.0;
        vec3 lightDir = normalize(light.Pos - point);
        float distanceLight = distance(light.Pos, point);
        SRay shadowRay = SRay(point + lightDir*EPSILON, lightDir);


        if(RayTrace(shadowRay,distanceLight,tmp_i,tmp_v,tmp_v))
        {
            shadow = 0.0;
        }


        float diffuse = max(dot(lightDir,norm),0.0);
        vec3 view = normalize(uCamera.Position - point);
        vec3 reflected = reflect(-view, norm);
        float specular = pow(max(dot(reflected,lightDir),0.0),m.LightCoeffs.w);
        return m.LightCoeffs.x * m.Color + m.LightCoeffs.y * diffuse * m.Color*shadow + m.LightCoeffs.z * specular;

    }

    return color;
}

void main(void)
{    
    InitObjects();

    uCamera = GetCamera();
    SRay ray = GenerateRay(uCamera);
    FragColor = vec4(RayTraceColor(ray),1.0);
}