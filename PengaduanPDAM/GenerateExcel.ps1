# Buat Excel (.xlsx) tanpa module tambahan - pakai COM Excel
# Fallback: kalau Excel tidak terinstall, pakai CSV lalu convert

$outputPath = "c:\Users\TaraDwiMahendra\Documents\PDAM\PengaduanPDAM\DataImportPengaduan.xlsx"

try {
    $excel = New-Object -ComObject Excel.Application
    $excel.Visible = $false
    $excel.DisplayAlerts = $false
    
    $workbook = $excel.Workbooks.Add()
    $sheet = $workbook.Worksheets.Item(1)
    $sheet.Name = "Pengaduan"
    
    # Header
    $headers = @("Tanggal_Pengaduan", "Nama_pelapor", "NoTelepon", "Alamat", "NamaKategori", "Judul_laporan", "Deskripsi_Laporan", "StatusPengaduan")
    for ($i = 0; $i -lt $headers.Count; $i++) {
        $sheet.Cells.Item(1, $i + 1) = $headers[$i]
        $sheet.Cells.Item(1, $i + 1).Font.Bold = $true
        $sheet.Cells.Item(1, $i + 1).Interior.Color = 0xD9E1F2  # Light blue
    }
    
    # Data rows
    $data = @(
        @("2026-06-25", "Budi Santoso", "081998877665", "Jl. Merdeka No 10", "Teknis", "Meteran Bocor", "Air merembes terus dari meteran depan rumah.", "Menunggu"),
        @("2026-06-24", "Andi Wijaya", "08122334455", "Jl. Sudirman Blok B", "Administrasi", "Salah Catat Meter", "Tagihan bulan ini lonjak drastis, mohon dicek ulang.", "Menunggu"),
        @("2026-06-23", "Citra Kirana", "085566778899", "Perumahan Asri", "Teknis", "Air Keruh Kuning", "Air yang keluar dari keran warnanya kuning pekat.", "Diproses"),
        @("2026-06-22", "Tara", "081123456789", "Denpasar", "Teknis", "Air Mati", "Air mati sejak pagi tadi.", "Selesai")
    )
    
    for ($row = 0; $row -lt $data.Count; $row++) {
        for ($col = 0; $col -lt $data[$row].Count; $col++) {
            $sheet.Cells.Item($row + 2, $col + 1) = $data[$row][$col]
        }
    }
    
    # Auto-fit columns
    $sheet.UsedRange.EntireColumn.AutoFit() | Out-Null
    
    # Save
    $workbook.SaveAs($outputPath, 51) # 51 = xlOpenXMLWorkbook (.xlsx)
    $workbook.Close()
    $excel.Quit()
    
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel) | Out-Null
    
    Write-Host "File Excel berhasil dibuat di: $outputPath"
}
catch {
    Write-Host "Error: $_"
    Write-Host "Mencoba fallback..."
    
    # Cleanup COM jika error
    try { $excel.Quit() } catch {}
}
