const express = require('express');
const mysql = require('mysql2');
const cors = require('cors');
const app = express();

app.use(express.json());
app.use(cors());

const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '112233',
    database: 'gimal'
});

db.connect((err) => {
    if (err) {
        console.error('DB 연결 실패함ㅇ:', err);
    } else {
        console.log('데이터베이스 연결 성공ㅇㅇ');
    }
});

app.post('/api/rank', (req, res) => {
    const { player_name, stage_level, reaction_ms } = req.body;
    const sql = "INSERT INTO ranking (player_name, stage_level, reaction_ms) VALUES (?, ?, ?)";
    const params = [player_name, stage_level, reaction_ms];

    db.query(sql, params, (err, result) => {
        if (err) {
            console.error("DB 저장 에러:", err);
            res.status(500).send('서버 에러: 저장 실패');
        } else {
            res.status(200).send('등록완료');
        }
    });
});

app.get('/api/rank', (req, res) => {
    const sql = 'SELECT * FROM ranking ORDER BY stage_level DESC, reaction_ms ASC LIMIT 100';

    db.query(sql, (err, results) => {
        if (err) {
            
            res.status(500).send('에러 발생했어요');
        } else {
            res.json(results);
        }
    });
});

app.listen(3000, () => {
    console.log('게임 서버 잘되는중 (포트: 3000)');
});