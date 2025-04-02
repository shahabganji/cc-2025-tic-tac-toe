function playSound() {
    clickSound.currentTime = 0;
    clickSound.play();
}

function toggleTheme() {
    document.body.classList.toggle("dark");
    document.body.classList.toggle("light");
}

function showConfetti(isWinner) {
    confetti({
        particleCount: 100,
        spread: 70,
        origin: {y: 0.6},
        colors: isWinner ? undefined : ['#ff0000']
    });
}
