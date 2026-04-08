# Luật Chơi MineSweeper (Dò Mìn)

## 1. Mục tiêu

Mục tiêu của trò chơi là mở tất cả các ô không chứa mìn mà không kích hoạt bất kỳ ô mìn nào.

---

## 2. Bàn chơi

- Trò chơi được chơi trên một lưới hình chữ nhật  
- Mỗi ô có thể:
  - Chứa mìn  
  - Hoặc là ô an toàn  

---

## 3. Thuộc tính của ô

Mỗi ô bao gồm các thuộc tính sau:

- **IsMine**: Xác định ô có chứa mìn hay không  
- **IsRevealed**: Xác định ô đã được mở hay chưa  
- **IsFlagged**: Xác định ô có được đánh dấu là nghi ngờ có mìn hay không  
- **AdjacentMines**: Số lượng mìn trong 8 ô lân cận  

---

## 4. Khởi tạo trò chơi

Khi bắt đầu:

1. Tạo bàn chơi với kích thước xác định (ví dụ: 9×9, 16×16)  
2. Đặt ngẫu nhiên một số lượng mìn cố định  
3. Tính số lượng mìn lân cận cho mỗi ô  

---

## 5. Hành động của người chơi

### 5.1 Mở ô (Click chuột trái)

#### Trường hợp 1: Ô có mìn
- Trò chơi kết thúc ngay lập tức (Game Over)

#### Trường hợp 2: Ô an toàn

- Nếu **AdjacentMines > 0**:
  - Chỉ mở ô được chọn  
  - Hiển thị số lượng mìn xung quanh  

- Nếu **AdjacentMines == 0**:
  - Mở ô đó  
  - Tự động mở các ô lân cận theo cơ chế lan vùng (flood fill)  

---

### 5.2 Đánh dấu cờ (Click chuột phải)

- Người chơi có thể đánh dấu ô nghi ngờ có mìn  
- Ô đã được đánh dấu sẽ không thể mở nếu chưa bỏ cờ  

---

## 6. Cơ chế lan vùng (Flood Fill)

Khi mở một ô có **AdjacentMines == 0**:

- Tất cả các ô lân cận sẽ được mở tự động  
- Nếu ô lân cận cũng có giá trị 0 → tiếp tục lan  
- Nếu ô có số (> 0) → chỉ mở, không lan tiếp  

---

## 7. Điều kiện thắng

Người chơi thắng khi:

> Tất cả các ô không chứa mìn đã được mở

Không cần phải đánh dấu tất cả các ô mìn.

---

## 8. Điều kiện thua

Người chơi thua khi:

> Mở phải ô có mìn

---

## 9. Luật nâng cao (tùy chọn)

### 9.1 Click đầu tiên an toàn
- Lần click đầu tiên luôn không trúng mìn  

### 9.2 Chording (mở nhanh)
- Nếu một ô đã mở có số, và số lượng cờ xung quanh bằng số đó:
  - Tự động mở các ô chưa được đánh dấu xung quanh  

---

## 10. Tóm tắt

Logic cốt lõi của trò chơi bao gồm:

1. Tạo bàn chơi  
2. Đặt mìn ngẫu nhiên  
3. Tính số mìn lân cận  
4. Xử lý mở ô (bao gồm lan vùng)  
5. Kiểm tra điều kiện thắng/thua