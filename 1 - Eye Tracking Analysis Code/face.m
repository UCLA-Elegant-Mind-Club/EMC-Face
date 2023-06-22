eyefile = "C:\Users\Celegans\Desktop\EEG\eye tracking\Nora_1024_10_24_F1_ThreeFace_Eye_Tracking Eye_Tracking.txt";
T = readtable(eyefile, 'Delimiter', ' ' );
T = T((T{:,2})>=122*1000+T{1,2} & (T{:,2})<=150*1000+T{1,2}, :);
figure('Position',[0,0,3840,2160])
hold on
xlim([0,3840])
ylim([0,2160])
slower=1;

for i = 1:size(T,1)-1
    h = plot(T{i+1,3},T{i,4},'ob','MarkerSize',7.5);
%     L = plot([T{i,3},T{i+1,3}], [T{i,4},T{i+1,4}],'b','LineWidth',0.75);
    set(gca, 'YDir','reverse')
    xlim([1500,2500])
    ylim([900,1300])
    pause(slower*(T{i+1,2}-T{i,2})/1000)
%     delete(h)
end