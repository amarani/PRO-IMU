/*
 * MPU6050.h
 *
 * Created: 11/13/2013 6:51:38 PM
 *  Author: Ahmad Amiri
 */ 


#ifndef MPU6050_H_
#define MPU6050_H_
#include "../I2C/I2C.h"
#include "../Utility/ByteArrayFloat.h"

namespace MPU6050{

#define MPU6050_GYRO_LSB_250 131.0
#define MPU6050_GYRO_LSB_500 65.5
#define MPU6050_GYRO_LSB_1000 32.8
#define MPU6050_GYRO_LSB_2000 16.4

#define MPU6050_GYRO_LSB_250_Scale 0.007633 // 1/131
#define MPU6050_GYRO_LSB_500_Scale 0.015267 // 1/65.5
#define MPU6050_GYRO_LSB_1000_Scale 0.030487  // 1/32.8
#define MPU6050_GYRO_LSB_2000_Scale 0.060975  // 1/16.4


#define MPU6050_ACCEL_LSB_2 16384.0
#define MPU6050_ACCEL_LSB_4 8192.0
#define MPU6050_ACCEL_LSB_8 4096.0
#define MPU6050_ACCEL_LSB_16 2048.0


#define MPU6050_DEVICE_ADRESS 0x68  

#define MPU6050_GYRO_FULL_SCALE_RANGE_250 0 
#define MPU6050_GYRO_FULL_SCALE_RANGE_500 1
#define MPU6050_GYRO_FULL_SCALE_RANGE_1000 2
#define MPU6050_GYRO_FULL_SCALE_RANGE_2000 3

#define MPU6050_ACCEL_FULL_SCALE_RANGE_2G 0
#define MPU6050_ACCEL_FULL_SCALE_RANGE_4G 1
#define MPU6050_ACCEL_FULL_SCALE_RANGE_8G 2
#define MPU6050_ACCEL_FULL_SCALE_RANGE_16G 3

#define MPU6050_REG_SELF_TEST_X 13
#define MPU6050_REG_SELF_TEST_Y 14
#define MPU6050_REG_SELF_TEST_Z 15
#define MPU6050_REG_SELF_TEST_A 16
#define MPU6050_REG_SMPLRT_DIV 25
#define MPU6050_REG_CONFIG 26
#define MPU6050_REG_GYRO_CONFIG 27
#define MPU6050_REG_ACCEL_CONFIG 28
#define MPU6050_REG_MOT_THR 31
#define MPU6050_REG_FIFO_EN 35
#define MPU6050_REG_I2C_MST_CTRL 36
#define MPU6050_REG_I2C_SLV0_ADDR 37
#define MPU6050_REG_I2C_SLV0_REG 38
#define MPU6050_REG_I2C_SLV0_CTRL 39
#define MPU6050_REG_I2C_SLV1_ADDR 40
#define MPU6050_REG_I2C_SLV1_REG 41
#define MPU6050_REG_I2C_SLV1_CTRL 42
#define MPU6050_REG_I2C_SLV2_ADDR 43
#define MPU6050_REG_I2C_SLV2_REG 44
#define MPU6050_REG_I2C_SLV2_CTRL 45
#define MPU6050_REG_I2C_SLV3_ADDR 46
#define MPU6050_REG_I2C_SLV3_REG 47
#define MPU6050_REG_I2C_SLV3_CTRL 48
#define MPU6050_REG_I2C_SLV4_ADDR 49
#define MPU6050_REG_I2C_SLV4_REG 50
#define MPU6050_REG_I2C_SLV4_DO 51
#define MPU6050_REG_I2C_SLV4_CTRL 52
#define MPU6050_REG_I2C_SLV4_DI 53
#define MPU6050_REG_I2C_MST_STATUS 54
#define MPU6050_REG_INT_PIN_CFG 55
#define MPU6050_REG_INT_ENABLE 56
#define MPU6050_REG_INT_STATUS 58
#define MPU6050_REG_ACCEL_XOUT_H 59
#define MPU6050_REG_ACCEL_XOUT_L 60
#define MPU6050_REG_ACCEL_YOUT_H 61
#define MPU6050_REG_ACCEL_YOUT_L 62
#define MPU6050_REG_ACCEL_ZOUT_H 63
#define MPU6050_REG_ACCEL_ZOUT_L 64
#define MPU6050_REG_TEMP_OUT_H 65
#define MPU6050_REG_TEMP_OUT_L 66
#define MPU6050_REG_GYRO_XOUT_H 67
#define MPU6050_REG_GYRO_XOUT_L 68
#define MPU6050_REG_GYRO_YOUT_H 69
#define MPU6050_REG_GYRO_YOUT_L 70
#define MPU6050_REG_GYRO_ZOUT_H 71
#define MPU6050_REG_GYRO_ZOUT_L 72
#define MPU6050_REG_EXT_SENS_DATA_00 73
#define MPU6050_REG_EXT_SENS_DATA_01 74
#define MPU6050_REG_EXT_SENS_DATA_02 75
#define MPU6050_REG_EXT_SENS_DATA_03 76
#define MPU6050_REG_EXT_SENS_DATA_04 77
#define MPU6050_REG_EXT_SENS_DATA_05 78
#define MPU6050_REG_EXT_SENS_DATA_06 79
#define MPU6050_REG_EXT_SENS_DATA_07 80
#define MPU6050_REG_EXT_SENS_DATA_08 81
#define MPU6050_REG_EXT_SENS_DATA_09 82
#define MPU6050_REG_EXT_SENS_DATA_10 83
#define MPU6050_REG_EXT_SENS_DATA_11 84
#define MPU6050_REG_EXT_SENS_DATA_12 85
#define MPU6050_REG_EXT_SENS_DATA_13 86
#define MPU6050_REG_EXT_SENS_DATA_14 87
#define MPU6050_REG_EXT_SENS_DATA_15 88
#define MPU6050_REG_EXT_SENS_DATA_16 89
#define MPU6050_REG_EXT_SENS_DATA_17 90
#define MPU6050_REG_EXT_SENS_DATA_18 91
#define MPU6050_REG_EXT_SENS_DATA_19 92
#define MPU6050_REG_EXT_SENS_DATA_20 93
#define MPU6050_REG_EXT_SENS_DATA_21 94
#define MPU6050_REG_EXT_SENS_DATA_22 95
#define MPU6050_REG_EXT_SENS_DATA_23 96
#define MPU6050_REG_I2C_SLV0_DO 99
#define MPU6050_REG_I2C_SLV1_DO 100
#define MPU6050_REG_I2C_SLV2_DO 101
#define MPU6050_REG_I2C_SLV3_DO 102
#define MPU6050_REG_I2C_MST_DELAY_CTRL 103
#define MPU6050_REG_SIGNAL_PATH_RESET 104
#define MPU6050_REG_MOT_DETECT_CTRL 105
#define MPU6050_REG_USER_CTRL 106
#define MPU6050_REG_PWR_MGMT_1 107
#define MPU6050_REG_PWR_MGMT_2 108
#define MPU6050_REG_FIFO_COUNTH 114
#define MPU6050_REG_FIFO_COUNTL 115
#define MPU6050_REG_FIFO_R_W 116
#define MPU6050_REG_WHO_AM_I 117
 
void Initialize();

unsigned char GetByte(unsigned char reg);
void WriteByte(unsigned char reg,unsigned char value);
void BurstRead(unsigned char startRegAddress,unsigned char count, unsigned  char * buffer);
}

#endif /* MPU6050_H_ */