/*
Navicat MySQL Data Transfer

Source Server         : mysql
Source Server Version : 50547
Source Host           : localhost:3306
Source Database       : sqlsugar4xtest

Target Server Type    : MYSQL
Target Server Version : 50547
File Encoding         : 65001

Date: 2017-06-26 00:54:10
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for codetable
-- ----------------------------
DROP TABLE IF EXISTS `codetable`;
CREATE TABLE `codetable` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(21) NOT NULL,
  `IsOk` varchar(11) DEFAULT NULL,
  `Guid` varchar(10) NOT NULL,
  `Decimal` decimal(10,0) NOT NULL,
  `DateTime` datetime DEFAULT NULL,
  `Dob2` double DEFAULT NULL,
  `A` varchar(10) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of codetable
-- ----------------------------

-- ----------------------------
-- Table structure for codetable2
-- ----------------------------
DROP TABLE IF EXISTS `codetable2`;
CREATE TABLE `codetable2` (
  `Id` int(11) NOT NULL,
  `Name` varchar(1) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of codetable2
-- ----------------------------

-- ----------------------------
-- Table structure for datatestinfo
-- ----------------------------
DROP TABLE IF EXISTS `datatestinfo`;
CREATE TABLE `datatestinfo` (
  `Int1` int(11) NOT NULL AUTO_INCREMENT,
  `Int2` int(11) DEFAULT NULL,
  `String` varchar(10) NOT NULL,
  `Decimal1` decimal(10,0) NOT NULL,
  `Decimal2` decimal(10,0) DEFAULT NULL,
  `Datetime2` datetime DEFAULT NULL,
  `Image1` blob NOT NULL,
  `Image2` blob,
  `Guid1` varchar(40) NOT NULL,
  `Guid2` varchar(40) DEFAULT NULL,
  `Money1` decimal(10,0) NOT NULL,
  `Money2` decimal(10,0) DEFAULT NULL,
  `Varbinary1` blob NOT NULL,
  `Varbinary2` blob,
  `Float1` double DEFAULT NULL,
  `Float2` double DEFAULT NULL,
  `Datetime1` datetime DEFAULT NULL,
  PRIMARY KEY (`Int1`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of datatestinfo
-- ----------------------------
INSERT INTO `datatestinfo` VALUES ('1', '6', 'string', '1', '2', '2017-06-26 00:00:00', 0x0102, 0x0203, '00000001-8a7e-d826-d1a2-294399909bd8', 'd8268a7e-a2d1-4329-9990-9bd801292415', '7', '8', 0x0405, null, '4', '4', '2017-06-26 00:00:00');

-- ----------------------------
-- Table structure for datatestinfo2
-- ----------------------------
DROP TABLE IF EXISTS `datatestinfo2`;
CREATE TABLE `datatestinfo2` (
  `PK` varchar(1) NOT NULL,
  `Bool1` bit(1) NOT NULL,
  `Bool2` bit(1) NOT NULL,
  `Text1` varchar(1) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of datatestinfo2
-- ----------------------------

-- ----------------------------
-- Table structure for school
-- ----------------------------
DROP TABLE IF EXISTS `school`;
CREATE TABLE `school` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of school
-- ----------------------------

-- ----------------------------
-- Table structure for student
-- ----------------------------
DROP TABLE IF EXISTS `student`;
CREATE TABLE `student` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `SchoolId` int(11) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=28352 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of student
-- ----------------------------
