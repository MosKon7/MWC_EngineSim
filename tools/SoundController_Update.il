
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0



// предупреждение: ЭТО НЕПОЛНЫЙ ДИЗАССЕМБЛЕРНЫЙ КОД, НЕПРИГОДНЫЙ ДЛЯ ТРАНСЛЯЦИИ



.class public auto ansi beforefieldinit SoundController
       extends [UnityEngine]UnityEngine.MonoBehaviour
{
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

} // end of class SoundController

// *********** ДИЗАССЕМБЛИРОВАНИЕ ЗАВЕРШЕНО ***********************
