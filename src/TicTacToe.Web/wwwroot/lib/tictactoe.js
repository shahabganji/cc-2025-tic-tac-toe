function playSound() {
    clickSound.currentTime = 0;
    clickSound.play();
}

function toggleTheme() {
    document.body.classList.toggle("dark");
    document.body.classList.toggle("light");
}

function showConfetti() {
    confetti({
        particleCount: 100,
        spread: 70,
        origin: {y: 0.6}
    });
}
