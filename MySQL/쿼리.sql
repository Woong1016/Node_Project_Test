CREATE TABLE ranking (
    id INT AUTO_INCREMENT PRIMARY KEY,
    player_name CHAR(50) NOT NULL,     
    stage_level INT NOT NULL,         
    reaction_ms INT NOT NULL,        
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP  
    );
    
    
    