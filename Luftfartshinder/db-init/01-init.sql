CREATE USER IF NOT EXISTS 'app'@'%' IDENTIFIED BY 'app_pw';
GRANT ALL PRIVILEGES ON Kartverketdb.* TO 'app'@'%';
GRANT ALL PRIVILEGES ON AuthKartverketdb.* TO 'app'@'%';
FLUSH PRIVILEGES;

CREATE DATABASE IF NOT EXISTS Kartverketdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE DATABASE IF NOT EXISTS AuthKartverketdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

