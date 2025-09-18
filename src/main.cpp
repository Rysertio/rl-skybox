#include "raylib.h"
#include "rlgl.h"

#define SCREEN_W 1280
#define SCREEN_H 720

int main(void) {
    InitWindow(SCREEN_W, SCREEN_H, "Raylib GLSL Clouds FPS");
    SetTargetFPS(60);
    rlDisableBackfaceCulling();

    // Camera in FPS style
    Camera3D camera = {0};
    camera.position = (Vector3){ 0.0f, 2.0f, 4.0f };
    camera.target = (Vector3){ 0.0f, 2.0f, 0.0f };
    camera.up = (Vector3){ 0.0f, 1.0f, 0.0f };
    camera.fovy = 60.0f;
    camera.projection = CAMERA_PERSPECTIVE;


    // Load cloud shader
    Shader cloudShader = LoadShader("src/cloud.vs", "src/cloud.fs");
    int timeLoc = GetShaderLocation(cloudShader, "uTime");
    int sunLoc  = GetShaderLocation(cloudShader, "uSunDir");

    // Skybox mesh (cube)
    Mesh cube = GenMeshCube(50.0f, 50.0f, 50.0f);
    Model skybox = LoadModelFromMesh(cube);
    skybox.materials[0].shader = cloudShader;

    Vector3 sunDir = { 0.5f, 0.8f, 0.3f }; // arbitrary sun direction

    DisableCursor(); // FPS mode mouse capture

    while (!WindowShouldClose()) {
        UpdateCamera(&camera, CAMERA_FIRST_PERSON);   // raylib handles WASD + mouse look

        float t = GetTime();
        SetShaderValue(cloudShader, timeLoc, &t, SHADER_UNIFORM_FLOAT);
        SetShaderValue(cloudShader, sunLoc, &sunDir, SHADER_UNIFORM_VEC3);

        BeginDrawing();
        ClearBackground(BLACK);

        BeginMode3D(camera);
            DrawModel(skybox, (Vector3){0,0,0}, 1.0f, WHITE);
        EndMode3D();

        DrawText("WASD + Mouse = Move/Look, ESC to quit", 10, 10, 20, RAYWHITE);
        DrawFPS(10, 40);
        EndDrawing();
    }

    UnloadModel(skybox);
    UnloadShader(cloudShader);
    CloseWindow();
    return 0;
}
