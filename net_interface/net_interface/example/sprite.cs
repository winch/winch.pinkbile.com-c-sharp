//.net interface builder output file

using System.Runtime.InteropServices;

public class DbPro
{

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Sprite@@YAXHHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void sprite(int a, int b, int c, int d);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetSprite@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite(int a, int b, int c);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Size@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void size_sprite(int a, int b, int c);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Scale@@YAXHM@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void scale_sprite(int a, float b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Stretch@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void stretch_sprite(int a, int b, int c);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Offset@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void offset_sprite(int a, int b, int c);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Mirror@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void mirror_sprite(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Flip@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void flip_sprite(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Delete@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void delete_sprite(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Paste@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void paste_sprite(int a, int b, int c);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Hide@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void hide_sprite(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Show@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void show_sprite(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?HideAllSprites@@YAXXZ", CallingConvention = CallingConvention.Cdecl)]
public static extern void hide_all_sprites();

[DllImport("DBProSpritesDebug.dll", EntryPoint="?ShowAllSprites@@YAXXZ", CallingConvention = CallingConvention.Cdecl)]
public static extern void show_all_sprites();

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetExist@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_exist(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetX@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_x(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetY@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_y(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetXOffset@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_offset_x(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetYOffset@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_offset_y(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetWidth@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_width(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetHeight@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_height(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetImage@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_image(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetXScale@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_scale_x(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetYScale@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_scale_y(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetMirrored@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_mirrored(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetFlipped@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_flipped(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetHit@@YAHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_hit(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetCollision@@YAHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_collision(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Rotate@@YAXHM@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void rotate_sprite(int a, float b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetAlpha@@YAXHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite_alpha(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetDiffuse@@YAXHHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite_diffuse(int a, int b, int c, int d);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?CreateAnimatedSprite@@YAXHPADHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void create_animated_sprite(int a, string b, int c, int d, int e);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetTextureCoordinates@@YAXHHMM@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite_texture_coord(int a, int b, float c, float d);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Play@@YAXHHHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void play_sprite(int a, int b, int c, int d);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetFrame@@YAXHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite_frame(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetImage@@YAXHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite_image(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Clone@@YAXHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void clone_sprite(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetAngle@@YAKH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern float sprite_angle(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetAlpha@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_alpha(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetRed@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_red(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetGreen@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_green(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetBlue@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_blue(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetFrame@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_frame(int a);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?Move@@YAXHM@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void move_sprite(int a, float b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?SetPriority@@YAXHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern void set_sprite_priority(int a, int b);

[DllImport("DBProSpritesDebug.dll", EntryPoint="?GetVisible@@YAHH@Z", CallingConvention = CallingConvention.Cdecl)]
public static extern int sprite_visible(int a);

}
