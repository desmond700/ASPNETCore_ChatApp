-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Feb 26, 2019 at 04:53 AM
-- Server version: 10.1.37-MariaDB
-- PHP Version: 7.3.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `chatdb`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `getOnlineId` (IN `username` VARCHAR(150), OUT `online_id` INT)  NO SQL
BEGIN
	SELECT o.online_id 
    INTO online_id  
    FROM is_online o
    WHERE o.username = username;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `is_online`
--

CREATE TABLE `is_online` (
  `online_id` int(11) NOT NULL,
  `username` varchar(150) NOT NULL,
  `connection_id` varchar(256) NOT NULL,
  `date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `is_online`
--

INSERT INTO `is_online` (`online_id`, `username`, `connection_id`, `date`) VALUES
(33, 'desmond700', 'tO87_QlEQxIgk7lN0dQRlg', '2019-02-25 22:50:05');

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `user_id` int(11) NOT NULL,
  `username` varchar(150) NOT NULL,
  `email` varchar(250) NOT NULL,
  `password` varchar(250) NOT NULL,
  `image` varchar(256) NOT NULL,
  `status` varchar(45) NOT NULL DEFAULT 'Offline',
  `date_joined` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`user_id`, `username`, `email`, `password`, `image`, `status`, `date_joined`) VALUES
(8, 'desmond700', 'desmond700@gmail.com', 'password123', '15027583_1430471596982184_305908304077287745_n.jpg', 'Offline', '2019-02-25 19:25:14'),
(10, 'johndoe123', 'johndoe123@gmail.com', 'johndoe', 'user.png', 'Offline', '2019-02-25 19:25:14'),
(11, 'alex23', 'alexwest23@yahoo.com', 'alexwest', 'user.png', 'Offline', '2019-02-25 20:33:07');

--
-- Triggers `user`
--
DELIMITER $$
CREATE TRIGGER `user_before_insert_trigger` BEFORE INSERT ON `user` FOR EACH ROW BEGIN
	IF(EXISTS(SELECT username, email FROM user
             WHERE username = NEW.username AND email = NEW.email)) THEN
    	SIGNAL SQLSTATE VALUE '45000' SET MESSAGE_TEXT='Insert failed due to duplicated insert values';
    END IF;
END
$$
DELIMITER ;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `is_online`
--
ALTER TABLE `is_online`
  ADD PRIMARY KEY (`online_id`),
  ADD KEY `username` (`username`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `is_online`
--
ALTER TABLE `is_online`
  MODIFY `online_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `is_online`
--
ALTER TABLE `is_online`
  ADD CONSTRAINT `is_online_ibfk_1` FOREIGN KEY (`username`) REFERENCES `user` (`username`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
