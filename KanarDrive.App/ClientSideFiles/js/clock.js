function changeTime() {
    const now = new Date();
    document.getElementById('clock').textContent = `${changeFormat(now.getHours())}:${changeFormat(now.getMinutes())}:${changeFormat(now.getSeconds())}`;
    setTimeout(changeTime, 500);
}
function changeFormat(n) {
    if(n<10) {
        return `0${n}`;
    }else{
        return n;
    }
}
changeTime();