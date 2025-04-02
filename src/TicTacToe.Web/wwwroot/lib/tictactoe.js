const board = document.getElementById("board");
const statusText = document.getElementById("statusText");
const clickSound = document.getElementById("clickSound");

let currentPlayer = "X";
let cells = Array(9).fill("");
let isGameOver = false;
let winningCombo = [];

function createBoard() {
    board.innerHTML = "";
    cells.forEach((cell, index) => {
        const div = document.createElement("div");
        div.classList.add("cell");

        if (cell === "X") div.classList.add("x");
        if (cell === "O") div.classList.add("o");
        if (winningCombo.includes(index)) div.classList.add("winner");

        div.dataset.index = index;
        div.textContent = cell;
        div.addEventListener("click", onCellClick);

        board.appendChild(div);
    });
}


function onCellClick(e) {
    const index = e.target.dataset.index;

    if (cells[index] || isGameOver) return;

    cells[index] = currentPlayer;
    playSound();
    createBoard();         // <--- re-render first
    checkWinner();         // <--- then add .winner to fresh elements
    currentPlayer = currentPlayer === "X" ? "O" : "X";
    if (!isGameOver) {
        statusText.textContent = `Current Turn: ${currentPlayer}`;
    }
    createBoard();
}

function checkWinner() {
    const wins = [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6]
    ];

    for (let combo of wins) {
        const [a, b, c] = combo;
        if (cells[a] && cells[a] === cells[b] && cells[b] === cells[c]) {
            isGameOver = true;
            statusText.textContent = `${cells[a]} wins!`;
            winningCombo = combo;

            // Confetti explosion!
            confetti({
                particleCount: 100,
                spread: 70,
                origin: { y: 0.6 }
            });

            return;
        }
    }

    if (cells.every(cell => cell)) {
        isGameOver = true;
        statusText.textContent = "It's a draw!";
    }
}

function playSound() {
    clickSound.currentTime = 0;
    clickSound.play();
}

function restartGame() {

    currentPlayer = "X";
    cells = Array(9).fill("");
    isGameOver = false;
    winningCombo = [];

    statusText.textContent = "Current Turn: X";

    createBoard();
}

function toggleTheme() {
    document.body.classList.toggle("dark");
    document.body.classList.toggle("light");
}

createBoard();
