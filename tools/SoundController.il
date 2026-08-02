
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0



// предупреждение: ЭТО НЕПОЛНЫЙ ДИЗАССЕМБЛЕРНЫЙ КОД, НЕПРИГОДНЫЙ ДЛЯ ТРАНСЛЯЦИИ



.class public auto ansi beforefieldinit SoundController
       extends [UnityEngine]UnityEngine.MonoBehaviour
{
  .field public class [UnityEngine]UnityEngine.AudioClip engineThrottle
  .field public float32 engineThrottleVolume
  .field public float32 engineThrottlePitchFactor
  .field public class [UnityEngine]UnityEngine.AudioClip engineNoThrottle
  .field public float32 engineNoThrottleVolume
  .field public float32 engineNoThrottlePitchFactor
  .field public class [UnityEngine]UnityEngine.AudioClip startEngine
  .field public float32 startEngineVolume
  .field public float32 startEnginePitch
  .field public class [UnityEngine]UnityEngine.GameObject enginePosition
  .field public class [UnityEngine]UnityEngine.AudioClip transmission
  .field public float32 transmissionVolume
  .field public float32 transmissionVolumeReverse
  .field public float32 transmissionSourcePitch
  .field public class [UnityEngine]UnityEngine.AudioClip brakeNoise
  .field public float32 brakeNoiseVolume
  .field public class [UnityEngine]UnityEngine.AudioClip skid
  .field public float32 skidVolume
  .field public float32 skidPitchFactor
  .field public class [UnityEngine]UnityEngine.AudioClip crashHiSpeed
  .field public float32 crashHighVolume
  .field public class [UnityEngine]UnityEngine.AudioClip crashLowSpeed
  .field public float32 crashLowVolume
  .field public class [UnityEngine]UnityEngine.AudioClip scrapeNoise
  .field public float32 scrapeNoiseVolume
  .field public class [UnityEngine]UnityEngine.AudioClip ABSTrigger
  .field public float32 ABSTriggerVolume
  .field public class [UnityEngine]UnityEngine.AudioClip shiftTrigger
  .field public float32 shiftTriggerVolume
  .field public class [UnityEngine]UnityEngine.AudioClip wind
  .field public float32 windVolume
  .field public class [UnityEngine]UnityEngine.AudioClip rollingNoiseGrass
  .field public class [UnityEngine]UnityEngine.AudioClip rollingNoiseSand
  .field public class [UnityEngine]UnityEngine.AudioClip rollingNoiseOffroad
  .field private class [UnityEngine]UnityEngine.AudioSource engineThrottleSource
  .field private class [UnityEngine]UnityEngine.AudioSource engineNoThrottleSource
  .field private class [UnityEngine]UnityEngine.AudioSource startEngineSource
  .field private class [UnityEngine]UnityEngine.AudioSource transmissionSource
  .field private class [UnityEngine]UnityEngine.AudioSource brakeNoiseSource
  .field private class [UnityEngine]UnityEngine.AudioSource[] skidSource
  .field private class [UnityEngine]UnityEngine.AudioSource crashHiSpeedSource
  .field private class [UnityEngine]UnityEngine.AudioSource crashLowSpeedSource
  .field private class [UnityEngine]UnityEngine.AudioSource scrapeNoiseSource
  .field private class [UnityEngine]UnityEngine.AudioSource ABSTriggerSource
  .field private class [UnityEngine]UnityEngine.AudioSource shiftTriggerSource
  .field private class [UnityEngine]UnityEngine.AudioSource windSource
  .field private class [UnityEngine]UnityEngine.AudioSource[] rollingNoiseSource
  .field public class CarController carController
  .custom instance void [UnityEngine]UnityEngine.HideInInspector::.ctor() = ( 01 00 00 00 ) 
  .field private class CarDynamics cardynamics
  .field private class Drivetrain drivetrain
  .field private class Axles axles
  .field private bool rimScraping
  .field private float32 volumeFactor
  .field private int32 i
  .field private int32 k
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Размер кода:       205 (0xcd)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.r4     0.34999999
    IL_0006:  stfld      float32 SoundController::engineThrottleVolume
    IL_000b:  ldarg.0
    IL_000c:  ldc.r4     1.
    IL_0011:  stfld      float32 SoundController::engineThrottlePitchFactor
    IL_0016:  ldarg.0
    IL_0017:  ldc.r4     0.34999999
    IL_001c:  stfld      float32 SoundController::engineNoThrottleVolume
    IL_0021:  ldarg.0
    IL_0022:  ldc.r4     1.
    IL_0027:  stfld      float32 SoundController::engineNoThrottlePitchFactor
    IL_002c:  ldarg.0
    IL_002d:  ldc.r4     1.
    IL_0032:  stfld      float32 SoundController::startEngineVolume
    IL_0037:  ldarg.0
    IL_0038:  ldc.r4     1.
    IL_003d:  stfld      float32 SoundController::startEnginePitch
    IL_0042:  ldarg.0
    IL_0043:  ldc.r4     0.5
    IL_0048:  stfld      float32 SoundController::transmissionVolume
    IL_004d:  ldarg.0
    IL_004e:  ldc.r4     0.5
    IL_0053:  stfld      float32 SoundController::transmissionVolumeReverse
    IL_0058:  ldarg.0
    IL_0059:  ldc.r4     1.
    IL_005e:  stfld      float32 SoundController::transmissionSourcePitch
    IL_0063:  ldarg.0
    IL_0064:  ldc.r4     0.2
    IL_0069:  stfld      float32 SoundController::brakeNoiseVolume
    IL_006e:  ldarg.0
    IL_006f:  ldc.r4     1.
    IL_0074:  stfld      float32 SoundController::skidVolume
    IL_0079:  ldarg.0
    IL_007a:  ldc.r4     1.
    IL_007f:  stfld      float32 SoundController::skidPitchFactor
    IL_0084:  ldarg.0
    IL_0085:  ldc.r4     0.75
    IL_008a:  stfld      float32 SoundController::crashHighVolume
    IL_008f:  ldarg.0
    IL_0090:  ldc.r4     0.69999999
    IL_0095:  stfld      float32 SoundController::crashLowVolume
    IL_009a:  ldarg.0
    IL_009b:  ldc.r4     1.
    IL_00a0:  stfld      float32 SoundController::scrapeNoiseVolume
    IL_00a5:  ldarg.0
    IL_00a6:  ldc.r4     0.2
    IL_00ab:  stfld      float32 SoundController::ABSTriggerVolume
    IL_00b0:  ldarg.0
    IL_00b1:  ldc.r4     1.
    IL_00b6:  stfld      float32 SoundController::shiftTriggerVolume
    IL_00bb:  ldarg.0
    IL_00bc:  ldc.r4     1.
    IL_00c1:  stfld      float32 SoundController::windVolume
    IL_00c6:  ldarg.0
    IL_00c7:  call       instance void [UnityEngine]UnityEngine.MonoBehaviour::.ctor()
    IL_00cc:  ret
  } // end of method SoundController::.ctor

  .method private hidebysig instance class [UnityEngine]UnityEngine.AudioSource 
          CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip clip,
                            bool loop,
                            bool playImmediately,
                            valuetype [UnityEngine]UnityEngine.Vector3 position) cil managed
  {
    // Размер кода:       189 (0xbd)
    .maxstack  49
    .locals init (class [UnityEngine]UnityEngine.GameObject V_0)
    IL_0000:  ldstr      "audio"
    IL_0005:  newobj     instance void [UnityEngine]UnityEngine.GameObject::.ctor(string)
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt   instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.GameObject::get_transform()
    IL_0011:  ldarg.0
    IL_0012:  call       instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.Component::get_transform()
    IL_0017:  callvirt   instance void [UnityEngine]UnityEngine.Transform::set_parent(class [UnityEngine]UnityEngine.Transform)
    IL_001c:  ldloc.0
    IL_001d:  callvirt   instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.GameObject::get_transform()
    IL_0022:  ldarg.s    position
    IL_0024:  callvirt   instance void [UnityEngine]UnityEngine.Transform::set_localPosition(valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0029:  ldloc.0
    IL_002a:  callvirt   instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.GameObject::get_transform()
    IL_002f:  call       valuetype [UnityEngine]UnityEngine.Quaternion [UnityEngine]UnityEngine.Quaternion::get_identity()
    IL_0034:  callvirt   instance void [UnityEngine]UnityEngine.Transform::set_localRotation(valuetype [UnityEngine]UnityEngine.Quaternion)
    IL_0039:  ldloc.0
    IL_003a:  ldtoken    [UnityEngine]UnityEngine.AudioSource
    IL_003f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0044:  callvirt   instance class [UnityEngine]UnityEngine.Component [UnityEngine]UnityEngine.GameObject::AddComponent(class [mscorlib]System.Type)
    IL_0049:  pop
    IL_004a:  ldloc.0
    IL_004b:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_0050:  ldarg.1
    IL_0051:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_clip(class [UnityEngine]UnityEngine.AudioClip)
    IL_0056:  ldloc.0
    IL_0057:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_005c:  ldc.i4.0
    IL_005d:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_playOnAwake(bool)
    IL_0062:  ldarg.2
    IL_0063:  brfalse    IL_0089

    IL_0068:  ldloc.0
    IL_0069:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_006e:  ldc.r4     0.0
    IL_0073:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0078:  ldloc.0
    IL_0079:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_007e:  ldc.i4.1
    IL_007f:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_0084:  br         IL_0095

    IL_0089:  ldloc.0
    IL_008a:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_008f:  ldc.i4.0
    IL_0090:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_0095:  ldarg.3
    IL_0096:  brfalse    IL_00a6

    IL_009b:  ldloc.0
    IL_009c:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_00a1:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_00a6:  ldloc.0
    IL_00a7:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_00ac:  ldc.r4     1.
    IL_00b1:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_spatialBlend(float32)
    IL_00b6:  ldloc.0
    IL_00b7:  callvirt   instance !!0 [UnityEngine]UnityEngine.GameObject::GetComponent<class [UnityEngine]UnityEngine.AudioSource>()
    IL_00bc:  ret
  } // end of method SoundController::CreateAudioSource

  .method private hidebysig instance void 
          Start() cil managed
  {
    // Размер кода:       668 (0x29c)
    .maxstack  72
    .locals init (valuetype [UnityEngine]UnityEngine.Vector3 V_0,
             class Wheel V_1,
             class Wheel[] V_2,
             int32 V_3,
             class Wheel V_4,
             class Wheel[] V_5,
             int32 V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance !!0 [UnityEngine]UnityEngine.Component::GetComponent<class CarController>()
    IL_0007:  stfld      class CarController SoundController::carController
    IL_000c:  ldarg.0
    IL_000d:  ldarg.0
    IL_000e:  call       instance !!0 [UnityEngine]UnityEngine.Component::GetComponent<class CarDynamics>()
    IL_0013:  stfld      class CarDynamics SoundController::cardynamics
    IL_0018:  ldarg.0
    IL_0019:  ldarg.0
    IL_001a:  call       instance !!0 [UnityEngine]UnityEngine.Component::GetComponent<class Drivetrain>()
    IL_001f:  stfld      class Drivetrain SoundController::drivetrain
    IL_0024:  ldarg.0
    IL_0025:  ldarg.0
    IL_0026:  call       instance !!0 [UnityEngine]UnityEngine.Component::GetComponent<class Axles>()
    IL_002b:  stfld      class Axles SoundController::axles
    IL_0030:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_0035:  stloc.0
    IL_0036:  ldarg.0
    IL_0037:  ldfld      class [UnityEngine]UnityEngine.GameObject SoundController::enginePosition
    IL_003c:  ldnull
    IL_003d:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_0042:  brfalse    IL_0058

    IL_0047:  ldarg.0
    IL_0048:  ldfld      class [UnityEngine]UnityEngine.GameObject SoundController::enginePosition
    IL_004d:  callvirt   instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.GameObject::get_transform()
    IL_0052:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Transform::get_position()
    IL_0057:  stloc.0
    IL_0058:  ldarg.0
    IL_0059:  ldarg.0
    IL_005a:  ldarg.0
    IL_005b:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::engineThrottle
    IL_0060:  ldc.i4.1
    IL_0061:  ldc.i4.1
    IL_0062:  ldloc.0
    IL_0063:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0068:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::engineThrottleSource
    IL_006d:  ldarg.0
    IL_006e:  ldarg.0
    IL_006f:  ldarg.0
    IL_0070:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::engineNoThrottle
    IL_0075:  ldc.i4.1
    IL_0076:  ldc.i4.1
    IL_0077:  ldloc.0
    IL_0078:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_007d:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::engineNoThrottleSource
    IL_0082:  ldarg.0
    IL_0083:  ldarg.0
    IL_0084:  ldarg.0
    IL_0085:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::transmission
    IL_008a:  ldc.i4.1
    IL_008b:  ldc.i4.1
    IL_008c:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_0091:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0096:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::transmissionSource
    IL_009b:  ldarg.0
    IL_009c:  ldarg.0
    IL_009d:  ldarg.0
    IL_009e:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::brakeNoise
    IL_00a3:  ldc.i4.1
    IL_00a4:  ldc.i4.1
    IL_00a5:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_00aa:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_00af:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::brakeNoiseSource
    IL_00b4:  ldarg.0
    IL_00b5:  ldarg.0
    IL_00b6:  ldarg.0
    IL_00b7:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::startEngine
    IL_00bc:  ldc.i4.1
    IL_00bd:  ldc.i4.0
    IL_00be:  ldloc.0
    IL_00bf:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_00c4:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::startEngineSource
    IL_00c9:  ldarg.0
    IL_00ca:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::startEngineSource
    IL_00cf:  ldarg.0
    IL_00d0:  ldfld      float32 SoundController::startEngineVolume
    IL_00d5:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_00da:  ldarg.0
    IL_00db:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::startEngineSource
    IL_00e0:  ldarg.0
    IL_00e1:  ldfld      float32 SoundController::startEnginePitch
    IL_00e6:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_pitch(float32)
    IL_00eb:  ldarg.0
    IL_00ec:  ldflda     class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_00f1:  ldarg.0
    IL_00f2:  ldfld      class Axles SoundController::axles
    IL_00f7:  ldfld      class Wheel[] Axles::allWheels
    IL_00fc:  ldlen
    IL_00fd:  conv.i4
    IL_00fe:  call       void [mscorlib]System.Array::Resize<class [UnityEngine]UnityEngine.AudioSource>(!!0[]&,
                                                                                                         int32)
    IL_0103:  ldarg.0
    IL_0104:  ldc.i4.0
    IL_0105:  stfld      int32 SoundController::i
    IL_010a:  ldarg.0
    IL_010b:  ldfld      class Axles SoundController::axles
    IL_0110:  ldfld      class Wheel[] Axles::allWheels
    IL_0115:  stloc.2
    IL_0116:  ldc.i4.0
    IL_0117:  stloc.3
    IL_0118:  br         IL_0159

    IL_011d:  ldloc.2
    IL_011e:  ldloc.3
    IL_011f:  ldelem.ref
    IL_0120:  stloc.1
    IL_0121:  ldarg.0
    IL_0122:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_0127:  ldarg.0
    IL_0128:  ldfld      int32 SoundController::i
    IL_012d:  ldarg.0
    IL_012e:  ldarg.0
    IL_012f:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::skid
    IL_0134:  ldc.i4.1
    IL_0135:  ldc.i4.1
    IL_0136:  ldloc.1
    IL_0137:  callvirt   instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.Component::get_transform()
    IL_013c:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Transform::get_localPosition()
    IL_0141:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0146:  stelem.ref
    IL_0147:  ldarg.0
    IL_0148:  dup
    IL_0149:  ldfld      int32 SoundController::i
    IL_014e:  ldc.i4.1
    IL_014f:  add
    IL_0150:  stfld      int32 SoundController::i
    IL_0155:  ldloc.3
    IL_0156:  ldc.i4.1
    IL_0157:  add
    IL_0158:  stloc.3
    IL_0159:  ldloc.3
    IL_015a:  ldloc.2
    IL_015b:  ldlen
    IL_015c:  conv.i4
    IL_015d:  blt        IL_011d

    IL_0162:  ldarg.0
    IL_0163:  ldarg.0
    IL_0164:  ldarg.0
    IL_0165:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::crashHiSpeed
    IL_016a:  ldc.i4.0
    IL_016b:  ldc.i4.0
    IL_016c:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_0171:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0176:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashHiSpeedSource
    IL_017b:  ldarg.0
    IL_017c:  ldarg.0
    IL_017d:  ldarg.0
    IL_017e:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::crashLowSpeed
    IL_0183:  ldc.i4.0
    IL_0184:  ldc.i4.0
    IL_0185:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_018a:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_018f:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashLowSpeedSource
    IL_0194:  ldarg.0
    IL_0195:  ldarg.0
    IL_0196:  ldarg.0
    IL_0197:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::scrapeNoise
    IL_019c:  ldc.i4.0
    IL_019d:  ldc.i4.0
    IL_019e:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_01a3:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_01a8:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_01ad:  ldarg.0
    IL_01ae:  ldarg.0
    IL_01af:  ldarg.0
    IL_01b0:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::ABSTrigger
    IL_01b5:  ldc.i4.0
    IL_01b6:  ldc.i4.0
    IL_01b7:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_01bc:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_01c1:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::ABSTriggerSource
    IL_01c6:  ldarg.0
    IL_01c7:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::ABSTriggerSource
    IL_01cc:  ldarg.0
    IL_01cd:  ldfld      float32 SoundController::ABSTriggerVolume
    IL_01d2:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_01d7:  ldarg.0
    IL_01d8:  ldarg.0
    IL_01d9:  ldarg.0
    IL_01da:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::shiftTrigger
    IL_01df:  ldc.i4.0
    IL_01e0:  ldc.i4.0
    IL_01e1:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_01e6:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_01eb:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::shiftTriggerSource
    IL_01f0:  ldarg.0
    IL_01f1:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::shiftTriggerSource
    IL_01f6:  ldarg.0
    IL_01f7:  ldfld      float32 SoundController::shiftTriggerVolume
    IL_01fc:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0201:  ldarg.0
    IL_0202:  ldarg.0
    IL_0203:  ldarg.0
    IL_0204:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::wind
    IL_0209:  ldc.i4.1
    IL_020a:  ldc.i4.1
    IL_020b:  call       valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_zero()
    IL_0210:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0215:  stfld      class [UnityEngine]UnityEngine.AudioSource SoundController::windSource
    IL_021a:  ldarg.0
    IL_021b:  ldflda     class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0220:  ldarg.0
    IL_0221:  ldfld      class Axles SoundController::axles
    IL_0226:  ldfld      class Wheel[] Axles::allWheels
    IL_022b:  ldlen
    IL_022c:  conv.i4
    IL_022d:  call       void [mscorlib]System.Array::Resize<class [UnityEngine]UnityEngine.AudioSource>(!!0[]&,
                                                                                                         int32)
    IL_0232:  ldarg.0
    IL_0233:  ldc.i4.0
    IL_0234:  stfld      int32 SoundController::i
    IL_0239:  ldarg.0
    IL_023a:  ldfld      class Axles SoundController::axles
    IL_023f:  ldfld      class Wheel[] Axles::allWheels
    IL_0244:  stloc.s    V_5
    IL_0246:  ldc.i4.0
    IL_0247:  stloc.s    V_6
    IL_0249:  br         IL_0290

    IL_024e:  ldloc.s    V_5
    IL_0250:  ldloc.s    V_6
    IL_0252:  ldelem.ref
    IL_0253:  stloc.s    V_4
    IL_0255:  ldarg.0
    IL_0256:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_025b:  ldarg.0
    IL_025c:  ldfld      int32 SoundController::i
    IL_0261:  ldarg.0
    IL_0262:  ldarg.0
    IL_0263:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::rollingNoiseGrass
    IL_0268:  ldc.i4.1
    IL_0269:  ldc.i4.0
    IL_026a:  ldloc.s    V_4
    IL_026c:  callvirt   instance class [UnityEngine]UnityEngine.Transform [UnityEngine]UnityEngine.Component::get_transform()
    IL_0271:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Transform::get_localPosition()
    IL_0276:  call       instance class [UnityEngine]UnityEngine.AudioSource SoundController::CreateAudioSource(class [UnityEngine]UnityEngine.AudioClip,
                                                                                                                bool,
                                                                                                                bool,
                                                                                                                valuetype [UnityEngine]UnityEngine.Vector3)
    IL_027b:  stelem.ref
    IL_027c:  ldarg.0
    IL_027d:  dup
    IL_027e:  ldfld      int32 SoundController::i
    IL_0283:  ldc.i4.1
    IL_0284:  add
    IL_0285:  stfld      int32 SoundController::i
    IL_028a:  ldloc.s    V_6
    IL_028c:  ldc.i4.1
    IL_028d:  add
    IL_028e:  stloc.s    V_6
    IL_0290:  ldloc.s    V_6
    IL_0292:  ldloc.s    V_5
    IL_0294:  ldlen
    IL_0295:  conv.i4
    IL_0296:  blt        IL_024e

    IL_029b:  ret
  } // end of method SoundController::Start

  .method private hidebysig instance void 
          Update() cil managed
  {
    // Размер кода:       206 (0xce)
    .maxstack  16
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class CarController SoundController::carController
    IL_0006:  ldnull
    IL_0007:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_000c:  brfalse    IL_0066

    IL_0011:  ldarg.0
    IL_0012:  ldfld      class CarController SoundController::carController
    IL_0017:  ldfld      bool CarController::ABSTriggered
    IL_001c:  brfalse    IL_0032

    IL_0021:  ldarg.0
    IL_0022:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::ABSTriggerSource
    IL_0027:  ldarg.0
    IL_0028:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::ABSTrigger
    IL_002d:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::PlayOneShot(class [UnityEngine]UnityEngine.AudioClip)
    IL_0032:  ldarg.0
    IL_0033:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::brakeNoiseSource
    IL_0038:  ldarg.0
    IL_0039:  ldfld      class CarController SoundController::carController
    IL_003e:  ldfld      float32 CarController::brake
    IL_0043:  ldarg.0
    IL_0044:  call       instance float32 SoundController::AverageWheelVelo()
    IL_0049:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_004e:  mul
    IL_004f:  ldc.r4     0.1
    IL_0054:  mul
    IL_0055:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_005a:  ldarg.0
    IL_005b:  ldfld      float32 SoundController::brakeNoiseVolume
    IL_0060:  mul
    IL_0061:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0066:  ldarg.0
    IL_0067:  ldfld      class Drivetrain SoundController::drivetrain
    IL_006c:  ldnull
    IL_006d:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_0072:  brfalse    IL_00cd

    IL_0077:  ldarg.0
    IL_0078:  ldfld      class Drivetrain SoundController::drivetrain
    IL_007d:  ldfld      bool Drivetrain::startEngine
    IL_0082:  brfalse    IL_00c2

    IL_0087:  ldarg.0
    IL_0088:  ldfld      class Drivetrain SoundController::drivetrain
    IL_008d:  ldfld      float32 Drivetrain::rpm
    IL_0092:  ldarg.0
    IL_0093:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0098:  ldfld      float32 Drivetrain::minRPM
    IL_009d:  bge.un     IL_00c2

    IL_00a2:  ldarg.0
    IL_00a3:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::startEngineSource
    IL_00a8:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_00ad:  brtrue     IL_00bd

    IL_00b2:  ldarg.0
    IL_00b3:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::startEngineSource
    IL_00b8:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_00bd:  br         IL_00cd

    IL_00c2:  ldarg.0
    IL_00c3:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::startEngineSource
    IL_00c8:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Stop()
    IL_00cd:  ret
  } // end of method SoundController::Update

  .method private hidebysig instance float32 
          AverageWheelVelo() cil managed
  {
    // Размер кода:       68 (0x44)
    .maxstack  3
    .locals init (float32 V_0,
             class Wheel V_1,
             class Wheel[] V_2,
             int32 V_3)
    IL_0000:  ldc.r4     0.0
    IL_0005:  stloc.0
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class Axles SoundController::axles
    IL_000c:  ldfld      class Wheel[] Axles::allWheels
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br         IL_002a

    IL_0019:  ldloc.2
    IL_001a:  ldloc.3
    IL_001b:  ldelem.ref
    IL_001c:  stloc.1
    IL_001d:  ldloc.0
    IL_001e:  ldloc.1
    IL_001f:  ldfld      float32 Wheel::wheelTireVelo
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.3
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.3
    IL_002a:  ldloc.3
    IL_002b:  ldloc.2
    IL_002c:  ldlen
    IL_002d:  conv.i4
    IL_002e:  blt        IL_0019

    IL_0033:  ldloc.0
    IL_0034:  ldarg.0
    IL_0035:  ldfld      class Axles SoundController::axles
    IL_003a:  ldfld      class Wheel[] Axles::allWheels
    IL_003f:  ldlen
    IL_0040:  conv.i4
    IL_0041:  conv.r4
    IL_0042:  div
    IL_0043:  ret
  } // end of method SoundController::AverageWheelVelo

  .method private hidebysig instance void 
          FixedUpdate() cil managed
  {
    // Размер кода:       1885 (0x75d)
    .maxstack  130
    .locals init (float32 V_0,
             float32 V_1,
             float32 V_2,
             float32 V_3,
             class Wheel V_4,
             class Wheel[] V_5,
             int32 V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0006:  ldnull
    IL_0007:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_000c:  brfalse    IL_004a

    IL_0011:  ldarg.0
    IL_0012:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0017:  ldfld      bool Drivetrain::shiftTriggered
    IL_001c:  brfalse    IL_004a

    IL_0021:  ldarg.0
    IL_0022:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::shiftTriggerSource
    IL_0027:  ldarg.0
    IL_0028:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::shiftTrigger
    IL_002d:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::PlayOneShot(class [UnityEngine]UnityEngine.AudioClip)
    IL_0032:  ldarg.0
    IL_0033:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0038:  ldc.i4.0
    IL_0039:  stfld      bool Drivetrain::shiftTriggered
    IL_003e:  ldarg.0
    IL_003f:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0044:  ldc.i4.1
    IL_0045:  stfld      bool Drivetrain::soundPlayed
    IL_004a:  call       bool [UnityEngine]UnityEngine.Application::get_isEditor()
    IL_004f:  brfalse    IL_0098

    IL_0054:  ldarg.0
    IL_0055:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::shiftTriggerSource
    IL_005a:  ldnull
    IL_005b:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_0060:  brfalse    IL_0076

    IL_0065:  ldarg.0
    IL_0066:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::shiftTriggerSource
    IL_006b:  ldarg.0
    IL_006c:  ldfld      float32 SoundController::shiftTriggerVolume
    IL_0071:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0076:  ldarg.0
    IL_0077:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::ABSTriggerSource
    IL_007c:  ldnull
    IL_007d:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_0082:  brfalse    IL_0098

    IL_0087:  ldarg.0
    IL_0088:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::ABSTriggerSource
    IL_008d:  ldarg.0
    IL_008e:  ldfld      float32 SoundController::ABSTriggerVolume
    IL_0093:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0098:  ldarg.0
    IL_0099:  ldfld      class Drivetrain SoundController::drivetrain
    IL_009e:  call       bool [UnityEngine]UnityEngine.Object::op_Implicit(class [UnityEngine]UnityEngine.Object)
    IL_00a3:  brfalse    IL_0269

    IL_00a8:  ldarg.0
    IL_00a9:  ldfld      class Drivetrain SoundController::drivetrain
    IL_00ae:  ldfld      float32 Drivetrain::rpm
    IL_00b3:  ldarg.0
    IL_00b4:  ldfld      class Drivetrain SoundController::drivetrain
    IL_00b9:  ldfld      float32 Drivetrain::maxRPM
    IL_00be:  div
    IL_00bf:  stloc.0
    IL_00c0:  ldarg.0
    IL_00c1:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::engineNoThrottleSource
    IL_00c6:  ldc.r4     1.
    IL_00cb:  ldarg.0
    IL_00cc:  ldfld      class Drivetrain SoundController::drivetrain
    IL_00d1:  ldfld      float32 Drivetrain::throttle
    IL_00d6:  sub
    IL_00d7:  ldarg.0
    IL_00d8:  ldfld      float32 SoundController::engineNoThrottleVolume
    IL_00dd:  mul
    IL_00de:  ldloc.0
    IL_00df:  mul
    IL_00e0:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_00e5:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_00ea:  ldarg.0
    IL_00eb:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::engineNoThrottleSource
    IL_00f0:  ldc.r4     0.5
    IL_00f5:  ldarg.0
    IL_00f6:  ldfld      float32 SoundController::engineNoThrottlePitchFactor
    IL_00fb:  ldloc.0
    IL_00fc:  mul
    IL_00fd:  add
    IL_00fe:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_pitch(float32)
    IL_0103:  ldarg.0
    IL_0104:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::engineThrottleSource
    IL_0109:  ldarg.0
    IL_010a:  ldfld      class Drivetrain SoundController::drivetrain
    IL_010f:  ldfld      float32 Drivetrain::throttle
    IL_0114:  ldarg.0
    IL_0115:  ldfld      float32 SoundController::engineThrottleVolume
    IL_011a:  mul
    IL_011b:  ldloc.0
    IL_011c:  mul
    IL_011d:  ldarg.0
    IL_011e:  ldfld      float32 SoundController::engineThrottleVolume
    IL_0123:  ldc.r4     0.2
    IL_0128:  mul
    IL_0129:  ldloc.0
    IL_012a:  mul
    IL_012b:  add
    IL_012c:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0131:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0136:  ldarg.0
    IL_0137:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::engineThrottleSource
    IL_013c:  ldc.r4     0.5
    IL_0141:  ldarg.0
    IL_0142:  ldfld      float32 SoundController::engineThrottlePitchFactor
    IL_0147:  ldloc.0
    IL_0148:  mul
    IL_0149:  add
    IL_014a:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_pitch(float32)
    IL_014f:  ldarg.0
    IL_0150:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0155:  ldfld      class Clutch Drivetrain::clutch
    IL_015a:  brfalse    IL_023c

    IL_015f:  ldarg.0
    IL_0160:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0165:  ldfld      class Clutch Drivetrain::clutch
    IL_016a:  callvirt   instance float32 Clutch::GetClutchPosition()
    IL_016f:  ldc.r4     0.0
    IL_0174:  beq        IL_022c

    IL_0179:  ldarg.0
    IL_017a:  ldfld      class Drivetrain SoundController::drivetrain
    IL_017f:  ldfld      float32 Drivetrain::ratio
    IL_0184:  ldc.r4     0.0
    IL_0189:  beq        IL_022c

    IL_018e:  ldarg.0
    IL_018f:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0194:  ldfld      float32 Drivetrain::differentialSpeed
    IL_0199:  ldc.r4     9.9999998e-03
    IL_019e:  mul
    IL_019f:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_01a4:  stloc.1
    IL_01a5:  ldarg.0
    IL_01a6:  ldfld      class Drivetrain SoundController::drivetrain
    IL_01ab:  ldfld      float32 Drivetrain::ratio
    IL_01b0:  ldarg.0
    IL_01b1:  ldfld      class Drivetrain SoundController::drivetrain
    IL_01b6:  ldfld      float32 Drivetrain::lastGearRatio
    IL_01bb:  div
    IL_01bc:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_01c1:  stloc.2
    IL_01c2:  ldarg.0
    IL_01c3:  ldfld      float32 SoundController::transmissionVolume
    IL_01c8:  stloc.3
    IL_01c9:  ldarg.0
    IL_01ca:  ldfld      class Drivetrain SoundController::drivetrain
    IL_01cf:  ldfld      float32 Drivetrain::ratio
    IL_01d4:  ldc.r4     0.0
    IL_01d9:  bge.un     IL_01e5

    IL_01de:  ldarg.0
    IL_01df:  ldfld      float32 SoundController::transmissionVolumeReverse
    IL_01e4:  stloc.3
    IL_01e5:  ldarg.0
    IL_01e6:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::transmissionSource
    IL_01eb:  ldloc.3
    IL_01ec:  ldc.r4     1.
    IL_01f1:  ldarg.0
    IL_01f2:  ldfld      class Drivetrain SoundController::drivetrain
    IL_01f7:  ldfld      float32 Drivetrain::throttle
    IL_01fc:  sub
    IL_01fd:  ldloc.3
    IL_01fe:  mul
    IL_01ff:  ldc.r4     0.25
    IL_0204:  mul
    IL_0205:  sub
    IL_0206:  ldloc.2
    IL_0207:  div
    IL_0208:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_020d:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0212:  ldarg.0
    IL_0213:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::transmissionSource
    IL_0218:  ldloc.1
    IL_0219:  ldarg.0
    IL_021a:  ldfld      float32 SoundController::transmissionSourcePitch
    IL_021f:  mul
    IL_0220:  ldloc.2
    IL_0221:  mul
    IL_0222:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_pitch(float32)
    IL_0227:  br         IL_023c

    IL_022c:  ldarg.0
    IL_022d:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::transmissionSource
    IL_0232:  ldc.r4     0.0
    IL_0237:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_023c:  ldarg.0
    IL_023d:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0242:  ldfld      bool Drivetrain::shiftTriggered
    IL_0247:  brfalse    IL_0269

    IL_024c:  ldarg.0
    IL_024d:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::shiftTriggerSource
    IL_0252:  ldarg.0
    IL_0253:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::shiftTrigger
    IL_0258:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::PlayOneShot(class [UnityEngine]UnityEngine.AudioClip)
    IL_025d:  ldarg.0
    IL_025e:  ldfld      class Drivetrain SoundController::drivetrain
    IL_0263:  ldc.i4.0
    IL_0264:  stfld      bool Drivetrain::shiftTriggered
    IL_0269:  ldarg.0
    IL_026a:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::windSource
    IL_026f:  ldnull
    IL_0270:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_0275:  brfalse    IL_02a7

    IL_027a:  ldarg.0
    IL_027b:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::windSource
    IL_0280:  ldarg.0
    IL_0281:  ldfld      class CarDynamics SoundController::cardynamics
    IL_0286:  ldfld      float32 CarDynamics::velo
    IL_028b:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_0290:  ldarg.0
    IL_0291:  ldfld      float32 SoundController::windVolume
    IL_0296:  mul
    IL_0297:  ldc.r4     6.0000001e-03
    IL_029c:  mul
    IL_029d:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_02a2:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_02a7:  ldarg.0
    IL_02a8:  ldc.i4.0
    IL_02a9:  stfld      int32 SoundController::k
    IL_02ae:  ldarg.0
    IL_02af:  ldc.i4.0
    IL_02b0:  stfld      bool SoundController::rimScraping
    IL_02b5:  ldarg.0
    IL_02b6:  ldfld      class Axles SoundController::axles
    IL_02bb:  ldfld      class Wheel[] Axles::allWheels
    IL_02c0:  stloc.s    V_5
    IL_02c2:  ldc.i4.0
    IL_02c3:  stloc.s    V_6
    IL_02c5:  br         IL_0751

    IL_02ca:  ldloc.s    V_5
    IL_02cc:  ldloc.s    V_6
    IL_02ce:  ldelem.ref
    IL_02cf:  stloc.s    V_4
    IL_02d1:  ldloc.s    V_4
    IL_02d3:  ldfld      bool Wheel::rimScraping
    IL_02d8:  brfalse    IL_034c

    IL_02dd:  ldarg.0
    IL_02de:  ldc.i4.1
    IL_02df:  stfld      bool SoundController::rimScraping
    IL_02e4:  ldarg.0
    IL_02e5:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_02ea:  ldloc.s    V_4
    IL_02ec:  ldfld      float32 Wheel::angularVelocity
    IL_02f1:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_02f6:  ldc.r4     9.9999998e-03
    IL_02fb:  mul
    IL_02fc:  ldloc.s    V_4
    IL_02fe:  ldfld      float32 Wheel::slipVelo
    IL_0303:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_0308:  ldc.r4     3.5e-02
    IL_030d:  mul
    IL_030e:  add
    IL_030f:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0314:  ldarg.0
    IL_0315:  ldfld      float32 SoundController::scrapeNoiseVolume
    IL_031a:  mul
    IL_031b:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0320:  ldarg.0
    IL_0321:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0326:  ldc.i4.1
    IL_0327:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_032c:  ldarg.0
    IL_032d:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0332:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_0337:  brtrue     IL_0347

    IL_033c:  ldarg.0
    IL_033d:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0342:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_0347:  br         IL_0358

    IL_034c:  ldarg.0
    IL_034d:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0352:  ldc.i4.0
    IL_0353:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_0358:  ldarg.0
    IL_0359:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_035e:  ldarg.0
    IL_035f:  ldfld      int32 SoundController::k
    IL_0364:  ldelem.ref
    IL_0365:  ldnull
    IL_0366:  call       bool [UnityEngine]UnityEngine.Object::op_Inequality(class [UnityEngine]UnityEngine.Object,
                                                                             class [UnityEngine]UnityEngine.Object)
    IL_036b:  brfalse    IL_073d

    IL_0370:  ldarg.0
    IL_0371:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_0376:  ldarg.0
    IL_0377:  ldfld      int32 SoundController::k
    IL_037c:  ldelem.ref
    IL_037d:  ldarg.0
    IL_037e:  ldfld      float32 SoundController::skidPitchFactor
    IL_0383:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_pitch(float32)
    IL_0388:  ldarg.0
    IL_0389:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_038e:  ldarg.0
    IL_038f:  ldfld      int32 SoundController::k
    IL_0394:  ldelem.ref
    IL_0395:  ldloc.s    V_4
    IL_0397:  ldfld      float32 Wheel::slipVelo
    IL_039c:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_03a1:  ldc.r4     8.75e-03
    IL_03a6:  mul
    IL_03a7:  ldc.r4     0.0
    IL_03ac:  ldarg.0
    IL_03ad:  ldfld      float32 SoundController::skidVolume
    IL_03b2:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp(float32,
                                                                       float32,
                                                                       float32)
    IL_03b7:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_03bc:  ldarg.0
    IL_03bd:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_03c2:  ldarg.0
    IL_03c3:  ldfld      int32 SoundController::k
    IL_03c8:  ldelem.ref
    IL_03c9:  callvirt   instance float32 [UnityEngine]UnityEngine.AudioSource::get_volume()
    IL_03ce:  ldc.r4     9.9999998e-03
    IL_03d3:  bgt.un     IL_03ef

    IL_03d8:  ldarg.0
    IL_03d9:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_03de:  ldarg.0
    IL_03df:  ldfld      int32 SoundController::k
    IL_03e4:  ldelem.ref
    IL_03e5:  ldc.r4     0.0
    IL_03ea:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_03ef:  ldloc.s    V_4
    IL_03f1:  ldfld      valuetype CarDynamics/SurfaceType Wheel::surfaceType
    IL_03f6:  brtrue     IL_0417

    IL_03fb:  ldarg.0
    IL_03fc:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0401:  ldarg.0
    IL_0402:  ldfld      int32 SoundController::k
    IL_0407:  ldelem.ref
    IL_0408:  ldc.r4     0.0
    IL_040d:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0412:  br         IL_073d

    IL_0417:  ldloc.s    V_4
    IL_0419:  ldfld      valuetype CarDynamics/SurfaceType Wheel::surfaceType
    IL_041e:  ldc.i4.1
    IL_041f:  bne.un     IL_04fc

    IL_0424:  ldarg.0
    IL_0425:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_042a:  ldarg.0
    IL_042b:  ldfld      int32 SoundController::k
    IL_0430:  ldelem.ref
    IL_0431:  ldarg.0
    IL_0432:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::rollingNoiseGrass
    IL_0437:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_clip(class [UnityEngine]UnityEngine.AudioClip)
    IL_043c:  ldarg.0
    IL_043d:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0442:  ldarg.0
    IL_0443:  ldfld      int32 SoundController::k
    IL_0448:  ldelem.ref
    IL_0449:  ldloc.s    V_4
    IL_044b:  ldfld      float32 Wheel::angularVelocity
    IL_0450:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_0455:  ldc.r4     9.9999998e-03
    IL_045a:  mul
    IL_045b:  ldloc.s    V_4
    IL_045d:  ldfld      float32 Wheel::slipVelo
    IL_0462:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_0467:  ldc.r4     3.5e-02
    IL_046c:  mul
    IL_046d:  add
    IL_046e:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0473:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0478:  ldarg.0
    IL_0479:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_047e:  ldarg.0
    IL_047f:  ldfld      int32 SoundController::k
    IL_0484:  ldelem.ref
    IL_0485:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_048a:  brtrue     IL_04a1

    IL_048f:  ldarg.0
    IL_0490:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0495:  ldarg.0
    IL_0496:  ldfld      int32 SoundController::k
    IL_049b:  ldelem.ref
    IL_049c:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_04a1:  ldarg.0
    IL_04a2:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_04a7:  ldarg.0
    IL_04a8:  ldfld      int32 SoundController::k
    IL_04ad:  ldelem.ref
    IL_04ae:  callvirt   instance float32 [UnityEngine]UnityEngine.AudioSource::get_volume()
    IL_04b3:  ldc.r4     9.9999998e-03
    IL_04b8:  ble        IL_04c9

    IL_04bd:  ldloc.s    V_4
    IL_04bf:  ldfld      bool Wheel::onGroundDown
    IL_04c4:  brtrue     IL_04e0

    IL_04c9:  ldarg.0
    IL_04ca:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_04cf:  ldarg.0
    IL_04d0:  ldfld      int32 SoundController::k
    IL_04d5:  ldelem.ref
    IL_04d6:  ldc.r4     0.0
    IL_04db:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_04e0:  ldarg.0
    IL_04e1:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_04e6:  ldarg.0
    IL_04e7:  ldfld      int32 SoundController::k
    IL_04ec:  ldelem.ref
    IL_04ed:  ldc.r4     0.0
    IL_04f2:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_04f7:  br         IL_073d

    IL_04fc:  ldloc.s    V_4
    IL_04fe:  ldfld      valuetype CarDynamics/SurfaceType Wheel::surfaceType
    IL_0503:  ldc.i4.2
    IL_0504:  bne.un     IL_05e1

    IL_0509:  ldarg.0
    IL_050a:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_050f:  ldarg.0
    IL_0510:  ldfld      int32 SoundController::k
    IL_0515:  ldelem.ref
    IL_0516:  ldarg.0
    IL_0517:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::rollingNoiseSand
    IL_051c:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_clip(class [UnityEngine]UnityEngine.AudioClip)
    IL_0521:  ldarg.0
    IL_0522:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0527:  ldarg.0
    IL_0528:  ldfld      int32 SoundController::k
    IL_052d:  ldelem.ref
    IL_052e:  ldloc.s    V_4
    IL_0530:  ldfld      float32 Wheel::angularVelocity
    IL_0535:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_053a:  ldc.r4     9.9999998e-03
    IL_053f:  mul
    IL_0540:  ldloc.s    V_4
    IL_0542:  ldfld      float32 Wheel::slipVelo
    IL_0547:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_054c:  ldc.r4     3.5e-02
    IL_0551:  mul
    IL_0552:  add
    IL_0553:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0558:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_055d:  ldarg.0
    IL_055e:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0563:  ldarg.0
    IL_0564:  ldfld      int32 SoundController::k
    IL_0569:  ldelem.ref
    IL_056a:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_056f:  brtrue     IL_0586

    IL_0574:  ldarg.0
    IL_0575:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_057a:  ldarg.0
    IL_057b:  ldfld      int32 SoundController::k
    IL_0580:  ldelem.ref
    IL_0581:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_0586:  ldarg.0
    IL_0587:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_058c:  ldarg.0
    IL_058d:  ldfld      int32 SoundController::k
    IL_0592:  ldelem.ref
    IL_0593:  callvirt   instance float32 [UnityEngine]UnityEngine.AudioSource::get_volume()
    IL_0598:  ldc.r4     9.9999998e-03
    IL_059d:  ble        IL_05ae

    IL_05a2:  ldloc.s    V_4
    IL_05a4:  ldfld      bool Wheel::onGroundDown
    IL_05a9:  brtrue     IL_05c5

    IL_05ae:  ldarg.0
    IL_05af:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_05b4:  ldarg.0
    IL_05b5:  ldfld      int32 SoundController::k
    IL_05ba:  ldelem.ref
    IL_05bb:  ldc.r4     0.0
    IL_05c0:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_05c5:  ldarg.0
    IL_05c6:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_05cb:  ldarg.0
    IL_05cc:  ldfld      int32 SoundController::k
    IL_05d1:  ldelem.ref
    IL_05d2:  ldc.r4     0.0
    IL_05d7:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_05dc:  br         IL_073d

    IL_05e1:  ldloc.s    V_4
    IL_05e3:  ldfld      valuetype CarDynamics/SurfaceType Wheel::surfaceType
    IL_05e8:  ldc.i4.3
    IL_05e9:  bne.un     IL_06c6

    IL_05ee:  ldarg.0
    IL_05ef:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_05f4:  ldarg.0
    IL_05f5:  ldfld      int32 SoundController::k
    IL_05fa:  ldelem.ref
    IL_05fb:  ldarg.0
    IL_05fc:  ldfld      class [UnityEngine]UnityEngine.AudioClip SoundController::rollingNoiseOffroad
    IL_0601:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_clip(class [UnityEngine]UnityEngine.AudioClip)
    IL_0606:  ldarg.0
    IL_0607:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_060c:  ldarg.0
    IL_060d:  ldfld      int32 SoundController::k
    IL_0612:  ldelem.ref
    IL_0613:  ldloc.s    V_4
    IL_0615:  ldfld      float32 Wheel::angularVelocity
    IL_061a:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_061f:  ldc.r4     9.9999998e-03
    IL_0624:  mul
    IL_0625:  ldloc.s    V_4
    IL_0627:  ldfld      float32 Wheel::slipVelo
    IL_062c:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_0631:  ldc.r4     3.5e-02
    IL_0636:  mul
    IL_0637:  add
    IL_0638:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_063d:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0642:  ldarg.0
    IL_0643:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0648:  ldarg.0
    IL_0649:  ldfld      int32 SoundController::k
    IL_064e:  ldelem.ref
    IL_064f:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_0654:  brtrue     IL_066b

    IL_0659:  ldarg.0
    IL_065a:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_065f:  ldarg.0
    IL_0660:  ldfld      int32 SoundController::k
    IL_0665:  ldelem.ref
    IL_0666:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_066b:  ldarg.0
    IL_066c:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0671:  ldarg.0
    IL_0672:  ldfld      int32 SoundController::k
    IL_0677:  ldelem.ref
    IL_0678:  callvirt   instance float32 [UnityEngine]UnityEngine.AudioSource::get_volume()
    IL_067d:  ldc.r4     9.9999998e-03
    IL_0682:  ble        IL_0693

    IL_0687:  ldloc.s    V_4
    IL_0689:  ldfld      bool Wheel::onGroundDown
    IL_068e:  brtrue     IL_06aa

    IL_0693:  ldarg.0
    IL_0694:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0699:  ldarg.0
    IL_069a:  ldfld      int32 SoundController::k
    IL_069f:  ldelem.ref
    IL_06a0:  ldc.r4     0.0
    IL_06a5:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_06aa:  ldarg.0
    IL_06ab:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_06b0:  ldarg.0
    IL_06b1:  ldfld      int32 SoundController::k
    IL_06b6:  ldelem.ref
    IL_06b7:  ldc.r4     0.0
    IL_06bc:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_06c1:  br         IL_073d

    IL_06c6:  ldloc.s    V_4
    IL_06c8:  ldfld      valuetype CarDynamics/SurfaceType Wheel::surfaceType
    IL_06cd:  ldc.i4.4
    IL_06ce:  bne.un     IL_073d

    IL_06d3:  ldarg.0
    IL_06d4:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_06d9:  ldarg.0
    IL_06da:  ldfld      int32 SoundController::k
    IL_06df:  ldelem.ref
    IL_06e0:  ldnull
    IL_06e1:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_clip(class [UnityEngine]UnityEngine.AudioClip)
    IL_06e6:  ldarg.0
    IL_06e7:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_06ec:  ldarg.0
    IL_06ed:  ldfld      int32 SoundController::k
    IL_06f2:  ldelem.ref
    IL_06f3:  ldc.r4     0.0
    IL_06f8:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_06fd:  ldarg.0
    IL_06fe:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_0703:  ldarg.0
    IL_0704:  ldfld      int32 SoundController::k
    IL_0709:  ldelem.ref
    IL_070a:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_070f:  brtrue     IL_0726

    IL_0714:  ldarg.0
    IL_0715:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::rollingNoiseSource
    IL_071a:  ldarg.0
    IL_071b:  ldfld      int32 SoundController::k
    IL_0720:  ldelem.ref
    IL_0721:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Stop()
    IL_0726:  ldarg.0
    IL_0727:  ldfld      class [UnityEngine]UnityEngine.AudioSource[] SoundController::skidSource
    IL_072c:  ldarg.0
    IL_072d:  ldfld      int32 SoundController::k
    IL_0732:  ldelem.ref
    IL_0733:  ldc.r4     0.0
    IL_0738:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_073d:  ldarg.0
    IL_073e:  dup
    IL_073f:  ldfld      int32 SoundController::k
    IL_0744:  ldc.i4.1
    IL_0745:  add
    IL_0746:  stfld      int32 SoundController::k
    IL_074b:  ldloc.s    V_6
    IL_074d:  ldc.i4.1
    IL_074e:  add
    IL_074f:  stloc.s    V_6
    IL_0751:  ldloc.s    V_6
    IL_0753:  ldloc.s    V_5
    IL_0755:  ldlen
    IL_0756:  conv.i4
    IL_0757:  blt        IL_02ca

    IL_075c:  ret
  } // end of method SoundController::FixedUpdate

  .method private hidebysig instance void 
          OnCollisionEnter(class [UnityEngine]UnityEngine.Collision collInfo) cil managed
  {
    // Размер кода:       343 (0x157)
    .maxstack  51
    .locals init (valuetype [UnityEngine]UnityEngine.Vector3 V_0,
             valuetype [UnityEngine]UnityEngine.Vector3 V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [UnityEngine]UnityEngine.ContactPoint[] [UnityEngine]UnityEngine.Collision::get_contacts()
    IL_0006:  ldlen
    IL_0007:  conv.i4
    IL_0008:  ldc.i4.0
    IL_0009:  ble        IL_0156

    IL_000e:  ldarg.1
    IL_000f:  callvirt   instance valuetype [UnityEngine]UnityEngine.ContactPoint[] [UnityEngine]UnityEngine.Collision::get_contacts()
    IL_0014:  ldc.i4.0
    IL_0015:  ldelema    [UnityEngine]UnityEngine.ContactPoint
    IL_001a:  call       instance class [UnityEngine]UnityEngine.Collider [UnityEngine]UnityEngine.ContactPoint::get_thisCollider()
    IL_001f:  callvirt   instance class [UnityEngine]UnityEngine.GameObject [UnityEngine]UnityEngine.Component::get_gameObject()
    IL_0024:  callvirt   instance int32 [UnityEngine]UnityEngine.GameObject::get_layer()
    IL_0029:  ldstr      "Wheel"
    IL_002e:  call       int32 [UnityEngine]UnityEngine.LayerMask::NameToLayer(string)
    IL_0033:  beq        IL_0156

    IL_0038:  ldarg.0
    IL_0039:  ldarg.1
    IL_003a:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Collision::get_relativeVelocity()
    IL_003f:  stloc.0
    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance float32 [UnityEngine]UnityEngine.Vector3::get_magnitude()
    IL_0047:  ldc.r4     0.1
    IL_004c:  mul
    IL_004d:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0052:  stfld      float32 SoundController::volumeFactor
    IL_0057:  ldarg.0
    IL_0058:  dup
    IL_0059:  ldfld      float32 SoundController::volumeFactor
    IL_005e:  ldc.r4     0.30000001
    IL_0063:  ldarg.1
    IL_0064:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Collision::get_relativeVelocity()
    IL_0069:  stloc.1
    IL_006a:  ldloca.s   V_1
    IL_006c:  call       instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_normalized()
    IL_0071:  ldarg.1
    IL_0072:  callvirt   instance valuetype [UnityEngine]UnityEngine.ContactPoint[] [UnityEngine]UnityEngine.Collision::get_contacts()
    IL_0077:  ldc.i4.0
    IL_0078:  ldelema    [UnityEngine]UnityEngine.ContactPoint
    IL_007d:  call       instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.ContactPoint::get_normal()
    IL_0082:  call       float32 [UnityEngine]UnityEngine.Vector3::Dot(valuetype [UnityEngine]UnityEngine.Vector3,
                                                                       valuetype [UnityEngine]UnityEngine.Vector3)
    IL_0087:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_008c:  add
    IL_008d:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0092:  mul
    IL_0093:  stfld      float32 SoundController::volumeFactor
    IL_0098:  ldarg.0
    IL_0099:  ldfld      float32 SoundController::volumeFactor
    IL_009e:  ldc.r4     0.89999998
    IL_00a3:  ble.un     IL_00e0

    IL_00a8:  ldarg.0
    IL_00a9:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashHiSpeedSource
    IL_00ae:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_00b3:  brtrue     IL_00e0

    IL_00b8:  ldarg.0
    IL_00b9:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashHiSpeedSource
    IL_00be:  ldarg.0
    IL_00bf:  ldfld      float32 SoundController::volumeFactor
    IL_00c4:  ldarg.0
    IL_00c5:  ldfld      float32 SoundController::crashHighVolume
    IL_00ca:  mul
    IL_00cb:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_00d0:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_00d5:  ldarg.0
    IL_00d6:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashHiSpeedSource
    IL_00db:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_00e0:  ldarg.0
    IL_00e1:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashLowSpeedSource
    IL_00e6:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_00eb:  brtrue     IL_0118

    IL_00f0:  ldarg.0
    IL_00f1:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashLowSpeedSource
    IL_00f6:  ldarg.0
    IL_00f7:  ldfld      float32 SoundController::volumeFactor
    IL_00fc:  ldarg.0
    IL_00fd:  ldfld      float32 SoundController::crashLowVolume
    IL_0102:  mul
    IL_0103:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0108:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_010d:  ldarg.0
    IL_010e:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::crashLowSpeedSource
    IL_0113:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_0118:  ldarg.0
    IL_0119:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_011e:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_0123:  brtrue     IL_0156

    IL_0128:  ldarg.0
    IL_0129:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_012e:  ldarg.0
    IL_012f:  ldarg.1
    IL_0130:  ldc.r4     1.
    IL_0135:  call       instance float32 SoundController::SetScrapeNoiseVolume(class [UnityEngine]UnityEngine.Collision,
                                                                                float32)
    IL_013a:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_013f:  ldarg.0
    IL_0140:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0145:  ldc.i4.0
    IL_0146:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_014b:  ldarg.0
    IL_014c:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0151:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_0156:  ret
  } // end of method SoundController::OnCollisionEnter

  .method private hidebysig instance void 
          OnCollisionExit() cil managed
  {
    // Размер кода:       40 (0x28)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool SoundController::rimScraping
    IL_0006:  brtrue     IL_0027

    IL_000b:  ldarg.0
    IL_000c:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0011:  ldc.r4     0.0
    IL_0016:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_001b:  ldarg.0
    IL_001c:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0021:  ldc.i4.0
    IL_0022:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_0027:  ret
  } // end of method SoundController::OnCollisionExit

  .method private hidebysig instance void 
          OnCollisionStay(class [UnityEngine]UnityEngine.Collision collInfo) cil managed
  {
    // Размер кода:       63 (0x3f)
    .maxstack  10
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  ldc.r4     10.
    IL_000d:  call       instance float32 SoundController::SetScrapeNoiseVolume(class [UnityEngine]UnityEngine.Collision,
                                                                                float32)
    IL_0012:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_volume(float32)
    IL_0017:  ldarg.0
    IL_0018:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_001d:  ldc.i4.1
    IL_001e:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::set_loop(bool)
    IL_0023:  ldarg.0
    IL_0024:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0029:  callvirt   instance bool [UnityEngine]UnityEngine.AudioSource::get_isPlaying()
    IL_002e:  brtrue     IL_003e

    IL_0033:  ldarg.0
    IL_0034:  ldfld      class [UnityEngine]UnityEngine.AudioSource SoundController::scrapeNoiseSource
    IL_0039:  callvirt   instance void [UnityEngine]UnityEngine.AudioSource::Play()
    IL_003e:  ret
  } // end of method SoundController::OnCollisionStay

  .method private hidebysig instance float32 
          SetScrapeNoiseVolume(class [UnityEngine]UnityEngine.Collision collInfo,
                               [opt] float32 factor) cil managed
  {
    .param [2] = int32(0x00000001)
    // Размер кода:       101 (0x65)
    .maxstack  19
    .locals init (float32 V_0,
             float32 V_1,
             valuetype [UnityEngine]UnityEngine.Vector3 V_2,
             valuetype [UnityEngine]UnityEngine.Vector3 V_3)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Collision::get_relativeVelocity()
    IL_0006:  stloc.2
    IL_0007:  ldloca.s   V_2
    IL_0009:  call       instance float32 [UnityEngine]UnityEngine.Vector3::get_magnitude()
    IL_000e:  ldarg.2
    IL_000f:  div
    IL_0010:  stloc.0
    IL_0011:  ldc.r4     0.0
    IL_0016:  stloc.1
    IL_0017:  ldarg.1
    IL_0018:  callvirt   instance valuetype [UnityEngine]UnityEngine.ContactPoint[] [UnityEngine]UnityEngine.Collision::get_contacts()
    IL_001d:  ldlen
    IL_001e:  conv.i4
    IL_001f:  ldc.i4.0
    IL_0020:  ble        IL_0055

    IL_0025:  ldc.r4     1.
    IL_002a:  ldarg.1
    IL_002b:  callvirt   instance valuetype [UnityEngine]UnityEngine.ContactPoint[] [UnityEngine]UnityEngine.Collision::get_contacts()
    IL_0030:  ldc.i4.0
    IL_0031:  ldelema    [UnityEngine]UnityEngine.ContactPoint
    IL_0036:  call       instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.ContactPoint::get_normal()
    IL_003b:  ldarg.1
    IL_003c:  callvirt   instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Collision::get_relativeVelocity()
    IL_0041:  stloc.3
    IL_0042:  ldloca.s   V_3
    IL_0044:  call       instance valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::get_normalized()
    IL_0049:  call       float32 [UnityEngine]UnityEngine.Vector3::Dot(valuetype [UnityEngine]UnityEngine.Vector3,
                                                                       valuetype [UnityEngine]UnityEngine.Vector3)
    IL_004e:  call       float32 [UnityEngine]UnityEngine.Mathf::Abs(float32)
    IL_0053:  sub
    IL_0054:  stloc.1
    IL_0055:  ldloc.0
    IL_0056:  ldloc.1
    IL_0057:  mul
    IL_0058:  ldarg.0
    IL_0059:  ldfld      float32 SoundController::scrapeNoiseVolume
    IL_005e:  mul
    IL_005f:  call       float32 [UnityEngine]UnityEngine.Mathf::Clamp01(float32)
    IL_0064:  ret
  } // end of method SoundController::SetScrapeNoiseVolume

} // end of class SoundController

// *********** ДИЗАССЕМБЛИРОВАНИЕ ЗАВЕРШЕНО ***********************
