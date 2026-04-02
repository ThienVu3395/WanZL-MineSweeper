# Luật Chơi MineSweeper (Dò Mìn)

## 1. Mục tiêu

Mục tiêu của trò chơi là mở toàn bộ các ô không chứa mìn mà không kích hoạt bất kỳ ô mìn nào.

---

## 2. Bàn chơi

- Trò chơi được chơi trên một lưới (grid)
- Mỗi ô có thể:
  - Chứa mìn
  - Hoặc là ô an toàn

---

## 3. Thuộc tính của ô

Mỗi ô có các thuộc tính:

- **IsMine**: Ô có chứa mìn hay không
- **IsRevealed**: Ô đã được mở hay chưa
- **IsFlagged**: Ô có được đánh dấu là nghi ngờ có mìn hay không
- **AdjacentMines**: Số lượng mìn ở 8 ô xung quanh

---

## 4. Khởi tạo trò chơi

Khi bắt đầu:

1. Tạo bàn chơi với kích thước xác định (ví dụ 9x9, 16x16)
2. Đặt ngẫu nhiên một số lượng mìn
3. Tính số lượng mìn xung quanh cho mỗi ô

---

## 5. Hành động của người chơi

### 5.1 Mở ô (Click chuột trái)

#### Trường hợp 1: Ô có mìn
- Trò chơi kết thúc (Game Over)

#### Trường hợp 2: Ô không có mìn

- Nếu **AdjacentMines > 0**:
  - Chỉ mở ô đó
  - Hiển thị số

- Nếu **AdjacentMines == 0**:
  - Mở ô đó
  - Tự động mở các ô xung quanh (lan vùng)

---

### 5.2 Đánh dấu cờ (Click chuột phải)

- Người chơi có thể đánh dấu ô nghi ngờ có mìn
- Ô đã đánh dấu sẽ không thể mở nếu chưa bỏ cờ

---

## 6. Cơ chế lan vùng (Flood Fill)

Khi mở một ô có **AdjacentMines == 0**:

- Tất cả các ô xung quanh sẽ được mở tự động
- Nếu ô xung quanh cũng có giá trị 0 → tiếp tục lan
- Nếu ô có số (> 0) → chỉ mở, không lan tiếp

---

## 7. Điều kiện thắng

Người chơi thắng khi:

> Tất cả các ô không có mìn đã được mở

Không cần phải đánh dấu hết tất cả mìn.

---

## 8. Điều kiện thua

Người chơi thua khi:

> Mở phải ô có mìn

---

## 9. Luật nâng cao (tùy chọn)

### 9.1 Click đầu tiên an toàn
- Click đầu tiên luôn không trúng mìn

### 9.2 Chording (mở nhanh)
- Nếu một ô đã mở có số, và số lượng cờ xung quanh bằng số đó:
  - Tự động mở các ô còn lại xung quanh

---

## 10. Tóm tắt

Logic cốt lõi của trò chơi bao gồm:

1. Tạo bàn chơi
2. Đặt mìn ngẫu nhiên
3. Tính số mìn xung quanh
4. Xử lý mở ô (kèm lan vùng)
5. Kiểm tra thắng/thua