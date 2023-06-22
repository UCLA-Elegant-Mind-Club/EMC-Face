% For eye tracking of sentences
% last edit: Yuanting 1020

%% Import Files
% eye tracking data:
eyefile = "C:\Users\Celegans\Desktop\EEG\eye tracking\Data\test1027(2)_10_27_F1_Sentence_Eye_Tracking_SM\test1027(2)_10_27_F1_Sentence_Eye_Tracking_SM_Eye_Tracking_Unix_Time.csv";
% the experiment result data from PsychoPy
resfile = "C:\Users\Celegans\Desktop\EEG\eye tracking\Data\test1027(2)_10_27_F1_Sentence_Eye_Tracking_SM\test1027(2)_10_27_F1_Sentence_Eye_Tracking_SM.csv";
% calibration file that doesn't really need to be changed
calibrationfile = "C:\Users\Celegans\Downloads\F1 Attention\eccentricity_monitor_calibration.csv";


%% Parameters
animation = 1;          % animation of eye track 1=on 0=off
save_video = 0;         % save animation as video 1=0n 0=off (Note: time consuming)
x_cord = 1;             % plotting x coordinates 1=on 0=off
slower = 3;             % makes animation and video slower. e.g. 1 = original speed, 5 = 5 times slower
height_adjust = 0;    % height adjust for eye tracking. set to 0 as default please.

%% Main
Traw = readtable(eyefile);
R = readtable(resfile,'Delimiter',',');
cali = readtable(calibrationfile);

starts = R{:,end};
ends = starts + R{:,end-2} + 0.1;
sentences = R(:,1);

% to pixels: 31.7355 = 3840/121 (px in x dir / monitor length in x dir)
centerX = table2array(cali(:,'centerx'));
centerY = table2array(cali(:,'centery'));
pixPerCm = cali{1,1}/cali{1,3};
sentenceX = repelem(centerX,5) * pixPerCm + 3840/2;
sentenceY = [centerY-10 centerY-5 centerY centerY+5 centerY+10] * pixPerCm + 2160/2;


% for trial = 1:size(starts,1)
for trial = 7
    T = Traw(Traw{:,1}>=starts(trial) & Traw{:,1}<=ends(trial), :);
    T{:,3} =  T{:,3}+ height_adjust;
    if animation
        figure('Position',[0,0,3840,2160])
        hold on
        xlim([0,3840])
        ylim([0,2160])
        
        split_sentences = split(sentences{trial,:}, '.');
        for sent = 1:5
            % in psychopy font height = 1 cm
            txt = text(sentenceX(sent),sentenceY(sent),split_sentences(sent),'FontSize',pixPerCm, 'FontUnits','pixels');
            set(txt,'visible','on','HorizontalAlignment','center','VerticalAlignment','middle')
        end

        if save_video
            vid = VideoWriter(sprintf('sentence %d slow %d.avi',trial, slower));
            vid.FrameRate = (size(T,1)/(T{end,1}-T{1,1}))/slower;
            open(vid)
        end
        
        for i = 1:size(T,1)-1
            h = plot(T{i+1,2},T{i,3},'ob','MarkerSize',3);
            title(sprintf('sentence # %d',trial))
            L = plot([T{i,2},T{i+1,2}], [T{i,3},T{i+1,3}],'b','LineWidth',0.8);
            set(gca, 'YDir','reverse')
            xlim([0,3840])
            ylim([0,2160])
            pause(slower*(T{i+1,1}-T{i,1}))
            if save_video
                frame = getframe(gcf);
                writeVideo(vid, frame);
            end
%             delete(h)
        end
        hold off
        if save_video
            close(vid)
        end
    end
    if x_cord
        figure('Position',[100,100,1500,500])
        plot(T{:,1},T{:,2}, 'o-','MarkerSize',3)
        title(sprintf('X coordinate of sentence # %d',trial))
        xlabel("Time (s)")
        ylabel("X coordinate (px)")
    end
end
